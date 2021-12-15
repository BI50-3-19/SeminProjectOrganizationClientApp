using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SeminClientApp
{
    public sealed partial class AddRowTableForm : Form
    {
        private readonly Button _addButton;
        private readonly TableForm _tableForm;

        public AddRowTableForm(TableForm table)
        {
            _tableForm = table;

            var columns = table.GetTableColumns();

            var index = 0;
            foreach (var column in columns)
                if (column != table.TableIdColumn)
                {
                    var relation = table.Relations.Where(x => x[1] == column);

                    var stringsEnumerable = relation as string[][] ?? relation.ToArray();
                    if (stringsEnumerable.Length != 0)
                    {
                        var relationInfo = stringsEnumerable.First().ToArray();

                        var comboBox = new ComboBox();
                        comboBox.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point,
                            204);
                        comboBox.Size = new Size(400, 50);
                        comboBox.Location = new Point(100, 50 * ++index);

                        var dataTable = new DataTable();
                        var query = $"SELECT {relationInfo[1]},{relationInfo[2]} FROM {relationInfo[0]};";
                        var adapter = new MySqlDataAdapter(query, table.Connection);
                        adapter.Fill(dataTable);

                        comboBox.DataSource = dataTable;
                        comboBox.DisplayMember = relationInfo[2];
                        comboBox.ValueMember = relationInfo[1];

                        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                        Controls.Add(comboBox);
                    }
                    else
                    {
                        var textBox = new TextBox();
                        textBox.Tag = column;
                        textBox.Text = column;
                        textBox.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point,
                            204);
                        textBox.Size = new Size(400, 50);
                        textBox.Location = new Point(100, 50 * ++index);
                        textBox.TextChanged += UpdateAddButtonStatus;
                        Controls.Add(textBox);
                    }
                }

            _addButton = new Button();
            _addButton.Text = @"Добавить";
            _addButton.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _addButton.Size = new Size(400, 50);
            _addButton.Location = new Point(100, 50 * (index + 3));
            _addButton.Click += AddButtonClick;
            UpdateAddButtonStatus(null, null);

            Controls.Add(_addButton);

            Text = $@"Добавление элемента в таблицу {table.Text}";

            InitializeComponent();
        }

        private string CreateQuery()
        {
            var textBoxList = Controls.OfType<TextBox>().ToArray();
            var comboBoxList = Controls.OfType<ComboBox>().ToArray();

            var index = 0;
            var queryParams = new string[textBoxList.Length + comboBoxList.Length][];

            foreach (var textBox in textBoxList)
            {
                queryParams[index] = new[] { textBox.Tag as string, textBox.Text };
                ++index;
            }

            foreach (var comboBox in comboBoxList)
            {
                queryParams[index] = new[] { comboBox.ValueMember, comboBox.SelectedValue.ToString() };
                ++index;
            }

            var columns = new string[queryParams.Length + 1];
            var values = new string[queryParams.Length + 1];

            for (var i = 0; i < queryParams.Length; ++i)
            {
                columns[i] = queryParams[i][0];
                if (queryParams[i][1].Contains("'")) throw new Exception("");
                values[i] = $"'{queryParams[i][1]}'";
            }

            columns[queryParams.Length] = _tableForm.TableIdColumn;
            values[queryParams.Length] = (_tableForm.GetMaxId() + 1).ToString();

            var query =
                $"INSERT INTO {_tableForm.TableName} ({string.Join(",", columns)}) VALUES ({string.Join(",", values)})";

            return query;
        }

        private void UpdateAddButtonStatus(object sender, EventArgs e)
        {
            _addButton.Enabled = Controls.OfType<TextBox>().All(x => x.Text != "");
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            string query;
            try
            {
                query = CreateQuery();
            }
            catch (Exception)
            {
                MessageBox.Show(@"Обнаружены недопустимые символы");
                return;
            }

            var command = new MySqlCommand(query, _tableForm.Connection);
            command.ExecuteNonQuery();
            Close();
        }
    }
}