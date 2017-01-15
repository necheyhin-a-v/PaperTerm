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
    /// Класс формы работников
    /// </summary>
    public partial class FormEmployees : Form
    {
        /// <summary>
        /// Событие, которое происходит при нажатии на кнопку выбрать специальности
        /// </summary>
        public event EventHandler ButtonSelectSpecialtiesClicked;
        /// <summary>
        /// Событие, которое происходит при нажатии на кнопку выбрать типы занятости
        /// </summary>
        public event EventHandler ButtonSelectEmploymentTypesClicked;

        private String DataBeforeEditing;
        private ContextMenuStrip contextMenuInfoEmploee;
        private IViewEmployee ViewEmployee;
        private IViewVacancy ViewVacancy;
        private bool AllowToFindJob = false;
        /// <summary>
        /// Конструктор формы работников
        /// </summary>
        public FormEmployees(IViewEmployee viewEmployee, IViewVacancy viewVacancy)
        {
            this.ViewEmployee = viewEmployee;
            this.ViewVacancy = viewVacancy;
            InitializeComponent();
            //Эмуляция изменения выбранной вкладки для подстройки размеров формы
            tcEmployees_SelectedIndexChanged(null, null);
            //Скрытие первого столбца
            this.dataGridInfo.RowHeadersVisible = false;
            //Подключить обработчик при нажатии на кнопку добавления специальностей
            this.buttonSelectSpecialties.Click += ButtonSelectSpecialtiesClicked;


            //Создание объектов контекстного меню  информации о работниках
            contextMenuInfoEmploee = new ContextMenuStrip();
            // Создание пунктов меню
            ToolStripMenuItem editMenuInfoItem = new ToolStripMenuItem("Редактировать");
            ToolStripMenuItem changeSelectedSpecialtiesMenuItem = new ToolStripMenuItem("Сменить предпочитаемые специальности");
            ToolStripMenuItem changeSelectedEmploymenTypesMenuItem = new ToolStripMenuItem("Сменить предпочитаемые типы занятости");
            ToolStripMenuItem suggestVacancyMenuItem = new ToolStripMenuItem("Подобрать вакансию");
            // Установка обработчиков событий для пунктов меню
            editMenuInfoItem.Click += EditMenuInfoItemClick;
            changeSelectedSpecialtiesMenuItem.Click += ChangeSelectedSpecialtiesMenuItemClick;
            changeSelectedEmploymenTypesMenuItem.Click += ChangeSelectedEmploymenTypesMenuItemClick;
            suggestVacancyMenuItem.Click += SuggestVacancyMenuItemClick;
            //Добавление пунктов меню в контекстное меню
            contextMenuInfoEmploee.Items.Add(editMenuInfoItem);
            contextMenuInfoEmploee.Items.Add(changeSelectedSpecialtiesMenuItem);
            contextMenuInfoEmploee.Items.Add(changeSelectedEmploymenTypesMenuItem);
            contextMenuInfoEmploee.Items.Add(suggestVacancyMenuItem);
        }
        /// <summary>
        /// Добавление контекстного меню для каждой строки таблицы
        /// "сведения о работнике"
        /// </summary>
        private void dataGridInfo_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //Перебрать все строки и установить для каждой контекстное меню
            for (int i = 0; i < dataGridInfo.RowCount; i++)
            {
                dataGridInfo.Rows[i].ContextMenuStrip = contextMenuInfoEmploee;
            }
        }
        /// <summary>
        /// Принять редактирование ячейки, заблокировать ее,
        /// внести изменения в базу и обновить информацию из базы данных
        /// </summary>
        private void dataGridInfo_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //Если пользователь еще не выбрал дату
            if (dateTimePicker.Visible)
                return;
            dataGridInfo.AllowUserToResizeColumns = true;
            dataGridInfo.ReadOnly = true;
            try
            {
                uint newExperience = 0;
                if (dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[6].Value != null)
                    newExperience = Convert.ToUInt32(dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[6].Value.ToString());
                ViewEmployee.ChangeEmployeeInfo(
                    (dataGridInfo.CurrentCell.ColumnIndex == 3) ? DataBeforeEditing :
                    dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[3].Value.ToString(),    //Старый пасспорт
                    dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[0].Value.ToString(),    //Имя                    
                    dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[1].Value.ToString(),    //Отчество
                    dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[2].Value.ToString(),    //Фамилия
                    dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[3].Value.ToString(),    //Новый пасспорт
                    dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[4].Value.ToString(),    //адрес
                    dataGridInfo.Rows[dataGridInfo.CurrentCell.RowIndex].Cells[5].Value.ToString(),    //телефон
                    newExperience,                                                                      //опыт работы                                                                                               //Была изменена дата у работника
                    (dataGridInfo.CurrentCell.ColumnIndex == 7) ? dateTimePicker.Value.Date.ToString() : "");//Статус работника
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                //MessageBox.Show("Ошибка при изменении данных");
            }
            finally
            {
                dateTimePicker.Visible = false;
                //Обновить информацию после изменения
                UpdateInfo(this.textBoxSearchInfo.Text);
            }
        }

        /// <summary>
        /// Обработчик события пункта меню редактировать информацию о работнике
        /// </summary>
        private void EditMenuInfoItemClick(object sender, EventArgs e)
        {
            //Установка dateTimePicker в ячейке для смены даты нахождения работы
            if (dataGridInfo.CurrentCell.ColumnIndex == 7)  //Смена поиска даты работы специалистом
            {
                dataGridInfo.AllowUserToResizeColumns = false;
                dateTimePicker.MaxDate = DateTime.Today;
                dateTimePicker.Value = DateTime.Today;
                //Установить положение dateTimePicker в ячейке
                dateTimePicker.Location = dataGridInfo.GetCellDisplayRectangle(
                    dataGridInfo.CurrentCell.ColumnIndex, dataGridInfo.CurrentCell.RowIndex, false).Location;
                Size cellSize = dataGridInfo.GetCellDisplayRectangle(
                     dataGridInfo.CurrentCell.ColumnIndex, dataGridInfo.CurrentCell.RowIndex, false).Size;
                dateTimePicker.Size = new Size(cellSize.Width, cellSize.Height + 1);
                dateTimePicker.Visible = true;
                dateTimePicker.Focus();
            }
            else
            {
                dateTimePicker.Visible = false;
            }
            dataGridInfo.ReadOnly = false;  //Открытие режима редактирования
            dataGridInfo.BeginEdit(false);  //Не выбирать все ячейки для редактирования
            DataBeforeEditing = dataGridInfo.CurrentCell.Value.ToString();
        }
        /// <summary>
        /// Обработчик события пункта меню сменить предпочитаемые специальности для текущего работника
        /// </summary>
        private void ChangeSelectedSpecialtiesMenuItemClick(object sender, EventArgs e)
        {
            MessageBox.Show("Изменение предпочитаемых специальностей");
        }
        private void ChangeSelectedEmploymenTypesMenuItemClick(object sender, EventArgs e)
        {
            MessageBox.Show("Изменение предпочитаемых типов занятости");
        }
        private void SuggestVacancyMenuItemClick(object sender, EventArgs e)
        {
            AllowToFindJob = true;
            tcEmployees.SelectedIndex = 2;
        }
        /// <summary>
        /// Выполнить регистрацию работника
        /// </summary>
        private void buttonCreate_Click(object sender, EventArgs e)
        {

            try
            {
                this.ViewEmployee.RegisterEmployee(
                this.textBoxPassport.Text,
                this.textBoxEmployerFirstName.Text,
                this.textBoxEmployerLastName.Text,
                this.textBoxEmployerSecondName.Text,
                this.textBoxEmployerAddress.Text,
                this.textBoxEmployerPhoneNumber.Text,
                this.textBoxEmployerExperience.Text);
                MessageBox.Show("Работник зарегистрирован");
                //Нажать на кнопку очищения формы
                buttonClear.PerformClick();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        /// <summary>
        /// Очистить форму регистрации работника
        /// </summary>
        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.textBoxPassport.Text = "";
            this.textBoxEmployerFirstName.Text = "";
            this.textBoxEmployerLastName.Text = "";
            this.textBoxEmployerSecondName.Text = "";
            this.textBoxEmployerAddress.Text = "";
            this.textBoxEmployerPhoneNumber.Text = "";
            this.textBoxEmployerExperience.Text = "";
        }
        /// <summary>
        /// Подстроить размеры формы под внутренний контент
        /// Запуск подгрузки данных при переходе на другую вкладку
        /// </summary>
        private void tcEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowToFindJob = false;
            switch (tcEmployees.SelectedIndex)
            {
                case 0: //Вкладка Регистрация
                    this.Size = new Size(380, 380);
                    break;
                case 1: //Вкладка сведения
                    this.Size = new Size(885, 580);
                    UpdateInfo(this.textBoxSearchInfo.Text);
                    break;
            }
        }

        /// <summary>
        /// Обновить таблицу с информацией о работниках
        /// Пытается применить фильтр для показа
        /// </summary>
        public void UpdateInfo(string filter = "")
        {
            try
            {
                this.dataGridInfo.SelectAll();
                this.dataGridInfo.ClearSelection();
                List<string[]> employees = ViewEmployee.GetEmployees(filter);
                if (employees.Count == 0)
                    throw new Exception("Нет результатов поиска");
                this.dataGridInfo.RowCount = employees.Count;
                int currentRow = 0;
                foreach (string[] currentEmployee in employees)
                {
                    //Добавить все элементы как текстовые
                    for (int i = 0; i < currentEmployee.Count() - 1; i++)
                        this.dataGridInfo.Rows[currentRow].Cells[i].Value = currentEmployee.ElementAt(i);
                    //Последний элемент добавить как check-box
                    //Создание новой ячейки-checkBox
                    DataGridViewCheckBoxCell comboBox = new DataGridViewCheckBoxCell();
                    comboBox.Value = Boolean.Parse(currentEmployee.ElementAt(currentEmployee.Length - 1));
                    //Установить новую ячейку в нужное место
                    dataGridInfo.Rows[currentRow].Cells[dataGridInfo.ColumnCount - 1] = comboBox;
                    currentRow++;
                }
            }
            catch (Exception err)
            {
                Console.Error.Write(err.Message);
                MessageBox.Show("Невозможно получить данные о работниках.\nПри выполнении поиска проверьте результаты");
            }
        }
        /// <summary>
        /// Очистить поиск информации о работниках
        /// </summary>
        private void buttonSearchInfo_Click(object sender, EventArgs e)
        {
            UpdateInfo(this.textBoxSearchInfo.Text);
        }
        /// <summary>
        /// Найти информацию о работниках
        /// </summary>
        private void buttonClearSearchInfo_Click(object sender, EventArgs e)
        {
            UpdateInfo(this.textBoxSearchInfo.Text = "");
        }
        /// <summary>
        /// Перенаправление сигнала о нажатии кнопки во внешнюю среду
        /// </summary>
        private void buttonSelectSpecialties_Click(object sender, EventArgs e)
        {
            ButtonSelectSpecialtiesClicked(sender, e);
        }
        /// <summary>
        /// Перенаправление сигнала о нажатии кнопки во внешнюю среду
        /// </summary>
        private void buttonSelectEmploymentTypes_Click(object sender, EventArgs e)
        {
            ButtonSelectEmploymentTypesClicked(sender, e);
        }
        /// <summary>
        /// Выделение ячейки при нажатии на контекстное меню
        /// </summary>
        private void dataGridInfo_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                dataGridInfo.CurrentCell = dataGridInfo[e.ColumnIndex, e.RowIndex];
            }
        }
        /// <summary>
        /// Использование быстрых клавиш для поиска и отмены поиска
        /// </summary>
        private void tcEmployees_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                switch (tcEmployees.SelectedIndex)
                {
                    case 0: //Вкладка Регистрация
                        this.buttonCreate.PerformClick();
                        break;
                    case 1: //Вкладка сведения
                        this.buttonSearchInfo.PerformClick();
                        break;
                    case 2: //Вкладка подбор вакансии
                        break;
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                switch (tcEmployees.SelectedIndex)
                {
                    case 0: //Вкладка Регистрация
                        this.buttonClear.PerformClick();
                        break;
                    case 1: //Вкладка сведения
                        this.buttonClearSearchInfo.PerformClick();
                        break;
                    case 2: //Вкладка подбор вакансии
                        break;
                }
            }
        }
        /// <summary>
        /// Изменение даты трудоустройства работника при окончании изменении даты
        /// </summary>
        private void dateTimePicker_Leave(object sender, EventArgs e)
        {
            //Скрыть dateTimePicker и симулировать изменение ячейки
            dateTimePicker.Visible = false;
            dataGridInfo_CellEndEdit(null, null);
        }
        /// <summary>
        /// Отмена перехода на вкладку "поиск работы"
        /// </summary>
        private void tcEmployees_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (AllowToFindJob == false && e.TabPageIndex == 2)
                e.Cancel = true;
        }
    }
}
