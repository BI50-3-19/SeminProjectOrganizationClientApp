using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SeminClientApp
{
    public sealed partial class EditRowTableForm : Form
    {
        private readonly int _id;
        private readonly TableForm _tableForm;
        private Button _editButton;

        public EditRowTableForm(TableForm table, int id)
        {
            _tableForm = table;
            _id = id;

            Text = $@"Редактирование элемента таблицы {table.Text}";

            InitializeComponent();

            RenderElementsProps();
        }

        private void RenderElementsProps()
        {
            var columns = _tableForm.GetTableColumns();
            var index = 0;

            var enumerableColumns = columns as string[] ?? columns.ToArray();

            var elementQuery =
                $@"SELECT {string.Join(",", enumerableColumns)} FROM {_tableForm.TableName} WHERE {_tableForm.TableName}.{_tableForm.TableIdColumn}='{_id}';";
            var elementDataTable = new DataTable();
            var elementAdapter = new MySqlDataAdapter(elementQuery, _tableForm.Connection);
            elementAdapter.Fill(elementDataTable);

            var idTextBox = new TextBox();
            idTextBox.Tag = _tableForm.TableIdColumn;
            idTextBox.Text = _id.ToString();
            idTextBox.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point,
                204);
            idTextBox.Size = new Size(400, 50);
            idTextBox.Location = new Point(100, 50 * ++index);
            idTextBox.Enabled = false;
            Controls.Add(idTextBox);

            string GetValue(string columnName)
            {
                foreach (DataColumn column in elementDataTable.Columns)
                    if (column.ColumnName == columnName)
                        return elementDataTable.Rows[0][column].ToString();

                return string.Empty;
            }

            foreach (var column in enumerableColumns)
            {
                var relation = _tableForm.Relations.Where(x => x[1] == column);

                var stringsEnumerable = relation as string[][] ?? relation.ToArray();
                if (stringsEnumerable.Length != 0)
                {
                    var relationInfo = stringsEnumerable.First().ToArray();

                    var comboBox = new ComboBox();
                    comboBox.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point,
                        204);
                    comboBox.Size = new Size(400, 50);
                    comboBox.Location = new Point(100, 50 * ++index);
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBox.Tag = column;

                    var dataTable = new DataTable();
                    var query = $"SELECT {relationInfo[1]},{relationInfo[2]} FROM {relationInfo[0]};";
                    var adapter = new MySqlDataAdapter(query, _tableForm.Connection);
                    adapter.Fill(dataTable);

                    comboBox.DisplayMember = relationInfo[2];
                    comboBox.ValueMember = relationInfo[1];
                    comboBox.DataSource = dataTable;

                    var rows = dataTable.Select($"{column} = {GetValue(column)}");

                    var rowIndex = dataTable.Rows.IndexOf(rows[0]);
                    var columnIndex = dataTable.Columns.IndexOf(relationInfo[1]);

                    comboBox.SelectedValue = dataTable.Rows[rowIndex][columnIndex];

                    Controls.Add(comboBox);
                }
                else
                {
                    if (column == _tableForm.TableIdColumn) continue;
                    var textBox = new TextBox();
                    textBox.Tag = column;
                    textBox.Text = GetValue(column);
                    textBox.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point,
                        204);
                    textBox.Size = new Size(400, 50);
                    textBox.Location = new Point(100, 50 * ++index);
                    textBox.TextChanged += UpdateEditButtonStatus;
                    Controls.Add(textBox);
                }
            }

            _editButton = new Button();
            _editButton.Text = @"Изменить";
            _editButton.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point, 204);
            _editButton.Size = new Size(400, 50);
            _editButton.Location = new Point(100, 50 * (index + 2));
            _editButton.Click += EditButtonClick;

            var deleteButton = new Button();
            deleteButton.Text = @"Удалить";
            deleteButton.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point, 204);
            deleteButton.Size = new Size(400, 50);
            deleteButton.Location = new Point(100, 50 * (index + 3));
            deleteButton.Click += DeleteButtonClick;

            Controls.Add(_editButton);
            Controls.Add(deleteButton);

            UpdateEditButtonStatus(null, null);
        }

        private void UpdateEditButtonStatus(object sender, EventArgs eventArgs)
        {
            _editButton.Enabled = Controls.OfType<TextBox>().All(x => x.Text != "");
        }

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            var query = $"DELETE FROM {_tableForm.TableName} WHERE {_tableForm.TableIdColumn}='{_id}';";
            var command = new MySqlCommand(query, _tableForm.Connection);
            command.ExecuteNonQuery();
            Close();
        }

        private void EditButtonClick(object sender, EventArgs e)
        {
            var updateParams = new List<string>();

            foreach (var textBox in Controls.OfType<TextBox>())
            {
                var column = textBox.Tag.ToString();
                var value = textBox.Text;
                updateParams.Add($"{column} = '{value}'");
            }

            foreach (var comboBox in Controls.OfType<ComboBox>())
            {
                var column = comboBox.Tag.ToString();
                var value = comboBox.SelectedValue.ToString();
                updateParams.Add($"{column} = {value}");
            }

            var query =
                $"UPDATE {_tableForm.TableName} SET {string.Join(",", updateParams)} WHERE {_tableForm.TableIdColumn} = {_id};";
            var command = new MySqlCommand(query, _tableForm.Connection);
            command.ExecuteNonQuery();
            Close();
        }
    }
}