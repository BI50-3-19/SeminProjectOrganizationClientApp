using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SeminClientApp
{
    public partial class AuthorizationForm : Form
    {
        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void loginTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonStatus();
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonStatus();
        }

        private void UpdateButtonStatus()
        {
            authorizationButton.Enabled = loginTextBox.Text != string.Empty && passwordTextBox.Text != string.Empty;
            clearButton.Enabled = loginTextBox.Text != string.Empty || passwordTextBox.Text != string.Empty;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            loginTextBox.Text = string.Empty;
            passwordTextBox.Text = string.Empty;
        }

        private void authorizationButton_Click(object sender, EventArgs e)
        {
            var connection =
                new MySqlConnection(
                    $"host=127.0.0.1;user={loginTextBox.Text};password={passwordTextBox.Text};SSL Mode=0;Database=project_org;");

            try
            {
                connection.Open();
            }
            catch (MySqlException except)
            {
                MessageBox.Show(except.Message, @"Ошибка авторизации.", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (Exception)
            {
                MessageBox.Show(@"Авторизация неудачна.", @"Ошибка авторизации.", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            finally
            {
                loginTextBox.Text = string.Empty;
                passwordTextBox.Text = string.Empty;
                UpdateButtonStatus();
            }

            var window = new TableListForm(connection);
            Hide();
            window.ShowDialog();
            Show();
        }
    }
}