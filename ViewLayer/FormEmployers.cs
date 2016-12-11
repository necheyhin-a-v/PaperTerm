﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViewLayer
{
    /// <summary>
    /// Интерфейс для взаимодействия со слоем представления
    /// </summary>
    public interface IViewEmployer
    {
        /// <summary>
        /// Зарегистрировать предпринимателя или предприятие
        /// </summary>
        /// <param name="name">Название организации</param>
        /// <param name="itn">ИНН предприятия</param>
        /// <param name="address">Адрес</param>
        /// <param name="phone"></param>
        void RegisterEmployer(string name, string itn, string address, string phone);
        /// <summary>
        /// Получить список работодателей
        /// name1, itn1, address1, phone1
        /// name2, itn2, address2, phone2
        /// ...
        /// </summary>
        /// <returns></returns>
        List<string[]> GetEmployers();
        /// <summary>
        /// Получить список вакансий
        /// В виде:
        /// имя вакансии, специальность, предприятие, требуемый опыт, тип занятости, оплата, описание
        /// </summary>
        List<string[]> GetVacancies();
    }


    /// <summary>
    /// Класс формы работы с работодателями
    /// </summary>
    public partial class FormEmployers : Form
    {
        ContextMenuStrip contextMenuInfoEmployer;
        private IViewEmployer View;
        /// <summary>
        /// Конструктор формы.
        /// Инициализирует все графические объекты формы
        /// </summary>
        public FormEmployers(IViewEmployer view)
        {
            this.View = view;
            InitializeComponent();
            //Установить размер формы начальный
            this.Size = new Size(377, 225);
            //Создание объекта контекстного меню
            contextMenuInfoEmployer = new ContextMenuStrip();
            // Создание пунктов меню
            ToolStripMenuItem editMenuItem = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem watchVacancyMenuItem = new ToolStripMenuItem("Просмотр вакансии");
            // Установка обработчиков событий для пунктов меню
            editMenuItem.Click += EditMenuItemClick;
            watchVacancyMenuItem.Click += WatchVacancyMenuItemClick;

            //Добавление пунктов меню в контекстное меню
            contextMenuInfoEmployer.Items.Add(editMenuItem);
            contextMenuInfoEmployer.Items.Add(watchVacancyMenuItem);

        }
        /// <summary>
        /// Обработчик событий для контекстного меню "Просмотр вакансий"
        /// </summary>
        void WatchVacancyMenuItemClick(object sender, EventArgs e)
        {
            MessageBox.Show("Просмотр вакансий");
            dataGridInfo.RowCount += 1;
        }
        /// <summary>
        /// Обработчик событий для контекстного меню "Редактирование"
        /// включает изменение текущей ячейки
        /// </summary>
        void EditMenuItemClick(object sender, EventArgs e)
        {
            dataGridInfo.ReadOnly = false;  //Открытие режима редактирования
            dataGridInfo.BeginEdit(false);  //Не выбирать все ячейки для редактирования
        }
        /// <summary>
        /// Подстроить размеры формы под внутренний контент
        /// Запуск подгрузки данных при переходе на другую вкладку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControlEmployersSelectedIndexChanged(object sender, EventArgs e)
        {
            switch(tabControlEmployers.SelectedIndex)
            {
                case 0: //Вкладка Регистрация
                    this.Size = new Size(377, 225);
                    break;
                case 1: //Вкладка сведения
                    this.Size = new Size(477, 400);
                    UpdateInfo();
                    break;
                case 2: //Вкладка вакансии
                    this.Size = new Size(777, 400);
                    UpdateVacancies();
                    break;
            }
        }
        /// <summary>
        /// Обновить таблицу с информацией о работодателях
        /// </summary>
        public void UpdateInfo()
        {
            //TODO: FormEmployers.UpdateInfo() добавить инициализацию фильтра "информация работодателей"
            this.dataGridInfo.SelectAll();
            this.dataGridInfo.ClearSelection();
            try
            {
                List<string[]> employers = View.GetEmployers();
                this.dataGridInfo.RowCount = employers.Count;
                int currentRow = 0;
                foreach (string[] currentEmployer in employers)
                {
                    for (int i = 0; i < currentEmployer.Count(); i++)
                        this.dataGridInfo.Rows[currentRow].Cells[i].Value = currentEmployer.ElementAt(i);
                    currentRow ++;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка в получении данных о работодателях");
            }
        }
        /// <summary>
        /// Обновление таблицы с информацией о вакансиях
        /// </summary>
        public void UpdateVacancies()
        {
            //TODO: FormEmployers.UpdateVacancies() добавить инициализацию фильтра "вакансии работодателей"
            this.dataGridVacancies.SelectAll();
            this.dataGridVacancies.ClearSelection();
            try
            {
                List<string[]> vacancies = View.GetVacancies();
                this.dataGridVacancies.RowCount = vacancies.Count;
                int currentRow = 0;
                foreach (string[] currentVacancy in vacancies)
                {
                    for (int i = 0; i < vacancies.Count(); i++)
                        this.dataGridVacancies.Rows[currentRow].Cells[i].Value = vacancies.ElementAt(i);
                    currentRow++;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка в получении данных о вакансиях");
            }
        }
        /// <summary>
        /// Блокировка ячейки в таблице "сведения о работодателе"
        /// при завершении редактирования ячейки
        /// </summary>
        private void dataGridInfo_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridInfo.ReadOnly = true;
        }
        /// <summary>
        /// Обработчик события на добавление новой строки в таблицу
        /// "сведения о работодателе"
        /// </summary>
        private void dataGridInfo_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //Перебрать все строки и установить для каждой контекстное меню
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                dataGridInfo.Rows[i].ContextMenuStrip = contextMenuInfoEmployer;
            }
        }
        /// <summary>
        /// Выделение ячейки в таблице "сведения о работодателе
        /// при нажании правой кнопки мыши
        /// </summary>
        private void dataGridInfo_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnIndex >= 0 && e.RowIndex >=0 )
            {
                dataGridInfo.CurrentCell = dataGridInfo[e.ColumnIndex, e.RowIndex];
            }
        }
        /// <summary>
        /// Запуск добавления новой вакансии
        /// </summary>
        private void buttonAddVacancy_Click(object sender, EventArgs e)
        {

            FormAddVacancy form = new FormAddVacancy();
            form.FormClosed += new FormClosedEventHandler(enableFormModerator);
            form.Show();
            this.Enabled = false;
        }
        /// <summary>
        /// Включение формы при закрытии формы "добавить вакансию"
        /// </summary>
        private void enableFormModerator(object sender, FormClosedEventArgs e)
        {
            this.Enabled = true;
        }
        /// <summary>
        /// Регистрация предприятия
        /// </summary>
        private void buttonAcceptRegistration_Click(object sender, EventArgs e)
        {
            try
            {
                this.View.RegisterEmployer(
                this.textBoxEmployerName.Text,
                this.textBoxITN.Text,
                this.textBoxEmployerAddress.Text,
                this.textBoxEmployerPhoneNumber.Text);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }
        /// <summary>
        /// Очистить поля формы регистрация
        /// </summary>
        private void buttonClearRegistration_Click(object sender, EventArgs e)
        {
            this.textBoxEmployerName.Text = "";
            this.textBoxITN.Text = "";
            this.textBoxEmployerAddress.Text = "";
            this.textBoxEmployerPhoneNumber.Text = "";
        }
    }
}
