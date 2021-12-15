using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SeminClientApp
{
    public partial class TableListForm : Form
    {
        private readonly MySqlConnection _connection;

        public TableListForm(MySqlConnection connection)
        {
            _connection = connection;
            InitializeComponent();
        }

        private void contractsButton_Click(object sender, EventArgs e)
        {
            var relations = new string[4][];
            relations[0] = new[] { "zakazchik", "ID_zakazchik", "FIO" };
            relations[1] = new[] { "rukovoditel", "ID_rukovoditel", "FIO" };
            relations[2] = new[] { "zackuchenie_dogovora", "ID_zaklichenie_dogovora", "Summa" };
            relations[3] = new[] { "vipltata_shtrafa", "ID_viplata_shtrafa", "Naimenovanie" };

            var queryParams = new string[7][];
            queryParams[0] = new[] { "dogovor", "Srok", "Срок сдачи" };
            queryParams[1] = new[] { "dogovor", "Summa", "Сумма" };
            queryParams[2] = new[] { "zakazchik", "FIO", "ФИО заказчика" };
            queryParams[3] = new[] { "rukovoditel", "FIO", "ФИО руководителя" };
            queryParams[4] = new[] { "zackuchenie_dogovora", "Summa", "Сумма заключения договора" };
            queryParams[5] = new[] { "vipltata_shtrafa", "Naimenovanie", "Штраф" };
            queryParams[6] = new[] { "vipltata_shtrafa", "Summa", "Сумма штрафа" };

            var form = new TableForm(_connection, "dogovor", "ID_dogovor", "Договоры", queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var relations = new string[2][];
            relations[0] = new[] { "rukovoditel", "ID_rukovoditel", "FIO" };
            relations[1] = new[] { "oborudovanie", "ID_oborudovanie", "Naimenovanie" };

            var queryParams = new string[3][];
            queryParams[0] = new[] { "otdel", "Nazvanie", "Название отдела" };
            queryParams[1] = new[] { "oborudovanie", "Naimenovanie", "Оборудование" };
            queryParams[2] = new[] { "rukovoditel", "FIO", "ФИО руководителя" };

            var form = new TableForm(_connection, "otdel", "ID_otdel", "Отделы", queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var relations = Array.Empty<string[]>();
            var queryParams = new string[1][];
            queryParams[0] = new[] { "oborudovanie", "Naimenovanie", "Наименование" };

            var form = new TableForm(_connection, "oborudovanie", "ID_oborudovanie", "Оборудование", queryParams,
                relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var relations = new string[3][];
            relations[0] = new[] { "sotrudniki", "ID_sotrudniki", "FIO" };
            relations[1] = new[] { "oborudovanie", "ID_oborudovanie", "Naimenovanie" };
            relations[2] = new[] { "dogovor", "ID_dogovor", "Summa" };

            var queryParams = new string[4][];
            queryParams[0] = new[] { "proekt", "Srok", "Срок сдачи" };
            queryParams[1] = new[] { "sotrudniki", "FIO", "Сотрудник" };
            queryParams[2] = new[] { "oborudovanie", "Naimenovanie", "Оборудование" };
            queryParams[3] = new[] { "dogovor", "Summa", "Сумма договора" };

            var form = new TableForm(_connection, "proekt", "ID_proekt", "Проекты", queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var relations = new string[1][];
            relations[0] = new[] { "otdel", "ID_otdel", "Nazvanie" };

            var queryParams = new string[3][];
            queryParams[0] = new[] { "kategoria_sotrudnikov", "Zarplata", "Зарплата" };
            queryParams[1] = new[] { "kategoria_sotrudnikov", "Naimenovanie", "Категория" };
            queryParams[2] = new[] { "otdel", "Nazvanie", "Отдел" };

            var form = new TableForm(_connection, "kategoria_sotrudnikov", "ID_kategoria_sotrudnikov",
                "Категории сотрудников", queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var relations = new string[1][];
            relations[0] = new[] { "otdel", "ID_otdel", "Nazvanie" };

            var queryParams = new string[3][];
            queryParams[0] = new[] { "rukovoditel", "FIO", "ФИО" };
            queryParams[1] = new[] { "rukovoditel", "Telefon", "Телефон" };
            queryParams[2] = new[] { "otdel", "Nazvanie", "Отдел" };

            var form = new TableForm(_connection, "rukovoditel", "ID_rukovoditel", "Руководитель", queryParams,
                relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var relations = new string[2][];
            relations[0] = new[] { "dogovor", "ID_dogovor", "Summa" };
            relations[1] = new[] { "proekt", "ID_proekt", "Srok" };

            var queryParams = new string[4][];
            queryParams[0] = new[] { "subpodryadnaya_organizatsia", "Summa", "Сумма" };
            queryParams[1] = new[] { "subpodryadnaya_organizatsia", "Crok", "Срок" };
            queryParams[2] = new[] { "dogovor", "Summa", "Сумма договора" };
            queryParams[3] = new[] { "proekt", "Srok", "Срок проекта" };

            var form = new TableForm(_connection, "subpodryadnaya_organizatsia", "ID_subpodryadnaya_organizatsia",
                "Субподрядная организация", queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var relations = Array.Empty<string[]>();
            var queryParams = new string[2][];
            queryParams[0] = new[] { "vipltata_shtrafa", "Naimenovanie", "Наименование" };
            queryParams[1] = new[] { "vipltata_shtrafa", "Summa", "Сумма" };

            var form = new TableForm(_connection, "vipltata_shtrafa", "ID_viplata_shtrafa", "Выплата штрафа",
                queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var relations = new string[2][];
            relations[0] = new[] { "kategoria_sotrudnikov", "ID_kategoria_sotrudnikov", "Naimenovanie" };
            relations[1] = new[] { "rukovoditel", "ID_rukovoditel", "FIO" };

            var queryParams = new string[5][];
            queryParams[0] = new[] { "sotrudniki", "FIO", "ФИО" };
            queryParams[1] = new[] { "sotrudniki", "Telefon", "Телефон" };
            queryParams[2] = new[] { "sotrudniki", "Data_rojdenia", "Дата рождения" };
            queryParams[3] = new[] { "kategoria_sotrudnikov", "Naimenovanie", "Категория" };
            queryParams[4] = new[] { "rukovoditel", "FIO", "Руководитель" };

            var form = new TableForm(_connection, "sotrudniki", "ID_sotrudniki", "Сотрудники", queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var relations = new string[2][];
            relations[0] = new[] { "dogovor", "ID_dogovor", "Summa" };
            relations[1] = new[] { "proekt", "ID_proekt", "Srok" };

            var queryParams = new string[4][];
            queryParams[0] = new[] { "vipolnenie_proekta", "Summarnaya_stoimost", "Суммарная стоимость" };
            queryParams[1] = new[] { "vipolnenie_proekta", "Srochnost_vipolneniya", "Срочность выполнения" };
            queryParams[2] = new[] { "dogovor", "Summa", "Сумма договора" };
            queryParams[3] = new[] { "proekt", "Srok", "Срок проекта" };

            var form = new TableForm(_connection, "vipolnenie_proekta", "ID_vipolnenie_proekta", "Выполнение проекта",
                queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var relations = new string[4][];
            relations[0] = new[] { "dogovor", "ID_dogovor", "Summa" };
            relations[1] = new[] { "proekt", "ID_proekt", "Srok" };
            relations[2] = new[] { "rukovoditel", "ID_rukovoditel", "FIO" };
            relations[3] = new[] { "subpodryadnaya_organizatsia", "ID_subpodryadnaya_organizatsia", "Crok" };

            var queryParams = new string[5][];
            queryParams[0] = new[] { "dogovor", "Summa", "Сумма договора" };
            queryParams[1] = new[] { "proekt", "Srok", "Срок проекта" };
            queryParams[2] = new[] { "rukovoditel", "FIO", "ФИО руководителя" };
            queryParams[3] = new[] { "subpodryadnaya_organizatsia", "Crok", "Срок сдачи для субподрядной организации" };
            queryParams[4] = new[] { "zackuchenie_dogovora", "Summa", "Сумма заключения договора" };

            var form = new TableForm(_connection, "zackuchenie_dogovora", "ID_zaklichenie_dogovora",
                "Заключение договора", queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var relations = Array.Empty<string[]>();
            var queryParams = new string[3][];
            queryParams[0] = new[] { "zakazchik", "FIO", "ФИО" };
            queryParams[1] = new[] { "zakazchik", "Naimenovanie_proekta", "Проект" };
            queryParams[2] = new[] { "zakazchik", "Telefon", "Телефон" };

            var form = new TableForm(_connection, "zakazchik", "ID_zakazchik", "Заказчик", queryParams, relations);
            Hide();
            form.ShowDialog();
            Show();
        }
    }
}