using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SeminClientApp
{
    public sealed partial class TableForm : Form
    {
        private static Panel _filtersPanel;
        private static Panel _findPanel;
        public readonly MySqlConnection Connection;
        private readonly string _defaultQuery;
        private readonly string[][] _queryParams;
        public readonly string[][] Relations;
        public readonly string TableIdColumn;
        public readonly string TableName;
        private string _lastQuery;

        public TableForm(MySqlConnection connection, string tableName, string tableIdColumn, string formTitle,
            string[][] queryParams, string[][] relations)
        {
            _defaultQuery = CreateDefaultQuery(tableName, tableIdColumn, queryParams, relations);

            Connection = connection;
            TableName = tableName;
            TableIdColumn = tableIdColumn;
            Relations = relations;
            _queryParams = queryParams;

            _filtersPanel = CreateFiltersPanel();
            _findPanel = CreateFindPanel();

            Text = formTitle;

            InitializeComponent();
            UpdateDataGridView();
        }

        private static string CreateDefaultQuery(string tableName, string tableIdColumn,
            IReadOnlyList<string[]> queryParams,
            IReadOnlyList<string[]> relations)
        {
            var query = new string[queryParams.Count + 1];
            query[0] = $"{tableName}.{tableIdColumn} AS 'ID'";
            for (var i = 0; i < queryParams.Count; ++i)
                query[i + 1] = $"{queryParams[i][0]}.{queryParams[i][1]} AS '{queryParams[i][2]}'";

            var conditionals = new string[relations.Count];
            var relationTables = new string[relations.Count + 1];

            relationTables[relations.Count] = tableName;

            for (var i = 0; i < relations.Count; ++i)
            {
                var relation = relations[i];
                conditionals[i] = $"{tableName}.{relation[1]}={relation[0]}.{relation[1]}";
                relationTables[i] = relation[0];
            }

            var selectString = string.Join(",", query);
            var relationTablesString = string.Join(",", relationTables);

            var queryString = $"SELECT {selectString} FROM {relationTablesString}";

            if (conditionals.Length > 0) queryString += " WHERE " + string.Join(" AND ", conditionals);

            return queryString;
        }

        private Panel CreateFiltersPanel()
        {
            var defaultFont = new Font("Microsoft Sans Serif", 12F,
                FontStyle.Regular, GraphicsUnit.Point, 204);
            var height = 20;

            var panel = new Panel();
            panel.Name = "FiltersPanel";

            var label = new Label();

            label.Text = @"Фильтры";
            label.Font = defaultFont;
            label.Size = new Size(280, 20);
            label.Location = new Point(30, height);
            label.TextAlign = ContentAlignment.TopCenter;

            panel.Controls.Add(label);

            height += 30;

            foreach (var relation in Relations)
            {
                var comboBox = new ComboBox();
                {
                    comboBox.Font = defaultFont;
                    comboBox.Size = new Size(280, 50);
                    comboBox.Location = new Point(30, height);

                    var dataTable = new DataTable();
                    var query = $"SELECT {relation[1]},{relation[2]} FROM {relation[0]};";
                    var adapter = new MySqlDataAdapter(query, Connection);
                    adapter.Fill(dataTable);

                    comboBox.DataSource = dataTable;
                    comboBox.DisplayMember = relation[2];
                    comboBox.ValueMember = relation[1];

                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                }

                var checkBox = new CheckBox();

                checkBox.Tag = comboBox;
                checkBox.Location = new Point(comboBox.Location.X - 20, comboBox.Location.Y);
                checkBox.CheckedChanged += delegate { UpdateDataGridView(); };


                comboBox.Tag = checkBox;
                comboBox.SelectedValueChanged += delegate
                {
                    if (checkBox.Checked)
                        UpdateDataGridView();
                    else
                        checkBox.Checked = true;
                };

                panel.Controls.Add(comboBox);
                panel.Controls.Add(checkBox);

                height += 50;
            }

            panel.Size = new Size(350, height);
            panel.BorderStyle = BorderStyle.Fixed3D;

            return panel;
        }

        private Panel CreateFindPanel()
        {
            var defaultFont = new Font("Microsoft Sans Serif", 12F,
                FontStyle.Regular, GraphicsUnit.Point, 204);
            var height = 20;

            var panel = new Panel();
            panel.Name = "FindPanel";

            var label = new Label();

            label.Text = @"Поиск";
            label.Font = defaultFont;
            label.Size = new Size(280, 20);
            label.Location = new Point(30, height);
            label.TextAlign = ContentAlignment.TopCenter;

            panel.Controls.Add(label);

            height += 30;

            foreach (var param in _queryParams)
            {
                var name = param[2];

                var checkBox = new CheckBox();
                checkBox.Tag = param;
                checkBox.Font = defaultFont;
                checkBox.Text = name;
                checkBox.Size = new Size(280, 30);
                checkBox.Location = new Point(30, height);
                checkBox.CheckedChanged += delegate { UpdateDataGridView(); };

                var textBox = new TextBox();
                textBox.Tag = checkBox;
                textBox.Font = defaultFont;
                textBox.Size = new Size(280, 50);
                textBox.Location = new Point(30, height + 25);
                textBox.TextChanged += delegate
                {
                    if (checkBox.Checked)
                        UpdateDataGridView();
                    else
                        checkBox.Checked = true;
                };

                panel.Controls.Add(textBox);
                panel.Controls.Add(checkBox);

                height += 70;
            }

            panel.Size = new Size(350, height);
            panel.BorderStyle = BorderStyle.Fixed3D;

            return panel;
        }

        private void UpdateDataGridView(bool force = false)
        {
            var query = _defaultQuery;

            if (filterCheckBox.Checked)
                foreach (var comboBox in _filtersPanel.Controls.OfType<ComboBox>())
                    if (comboBox.Tag is CheckBox checkBox && checkBox.Checked)
                    {
                        if (query.Contains("WHERE"))
                            query += $" AND {TableName}.{comboBox.ValueMember}='{comboBox.SelectedValue}'";
                        else
                            query += $" WHERE {TableName}.{comboBox.ValueMember}='{comboBox.SelectedValue}'";
                    }

            if (findCheckBox.Checked)
                foreach (var textBox in _findPanel.Controls.OfType<TextBox>())
                {
                    if (!(textBox.Tag is CheckBox checkBox) || !checkBox.Checked) continue;
                    if (!(checkBox.Tag is string[] param)) continue;
                    if (textBox.Text.Contains("'"))
                    {
                        MessageBox.Show(@"Обнаружены недопустимые символы.");
                        return;
                    }

                    if (query.Contains("WHERE"))
                        query += $" AND {param[0]}.{param[1]} LIKE '%{textBox.Text}%'";
                    else
                        query += $" WHERE {param[0]}.{param[1]} LIKE '%{textBox.Text}%'";
                }

            if (_lastQuery != null && !force && query == _lastQuery) return;

            var adapter = new MySqlDataAdapter(query, Connection);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;

            _lastQuery = query;
        }

        public IEnumerable<string> GetTableColumns()
        {
            var query = $"DESCRIBE {TableName};";
            var adapter = new MySqlDataAdapter(query, Connection);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            var columns = new string[dataTable.Rows.Count];

            for (var i = 0; i < dataTable.Rows.Count; ++i) columns[i] = dataTable.Rows[i][0].ToString();

            return columns;
        }

        public int GetMaxId()
        {
            var query = $"SELECT MAX({TableIdColumn}) from {TableName};";
            var adapter = new MySqlDataAdapter(query, Connection);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return int.Parse(dataTable.Rows[0][0].ToString());
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var addRowTableForm = new AddRowTableForm(this);
            addRowTableForm.ShowDialog();
            UpdateDataGridView(true);
        }

        private Point GetPanelLocation()
        {
            var offset = 0;

            if (Controls.Contains(_filtersPanel)) offset += _filtersPanel.Size.Height + 50;

            if (Controls.Contains(_findPanel)) offset += _findPanel.Size.Height + 50;

            return new Point(1300, offset);
        }

        private void enableFilterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (filterCheckBox.Checked)
            {
                _filtersPanel.Location = GetPanelLocation();
                Controls.Add(_filtersPanel);
                UpdateFormSize();
            }
            else
            {
                Controls.Remove(_filtersPanel);
                UpdateFormSize();
            }

            UpdateDataGridView();
        }

        private void UpdateFormSize()
        {
            if (!Controls.Contains(_filtersPanel) && !Controls.Contains(_findPanel))
                Size = new Size(1280, 720);
            else
                Size = new Size(1650, 720);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var editRowTableForm =
                new EditRowTableForm(this, int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
            editRowTableForm.ShowDialog();
            UpdateDataGridView(true);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void findCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (findCheckBox.Checked)
            {
                _findPanel.Location = GetPanelLocation();
                Controls.Add(_findPanel);
                UpdateFormSize();
            }
            else
            {
                Controls.Remove(_findPanel);
                UpdateFormSize();
            }

            UpdateDataGridView();
        }
    }
}