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
    public partial class FormSelectEmploymentTypes : Form
    {
        private IViewEmployee ViewEployee;
        /// <summary>
        /// Конструктор динамической формы
        /// </summary>
        public FormSelectEmploymentTypes(IViewEmployee viewEmployee, IViewSpecialty viewSpecialty)
        {
            this.ViewEployee = viewEmployee;
            InitializeComponent();
            List<string> employmentTypes = viewSpecialty.GetEmploymentTypes();
            //Настройка таблицы в ширину
            this.tableLayoutPanelTop.ColumnCount = 4;
            //Добавление последовательно всех специальностей в виде checkBox
            foreach (string item in employmentTypes)
            {
                CheckBox currentCheckBox = new CheckBox();
                currentCheckBox.Text = item;
                this.tableLayoutPanelTop.Controls.Add(currentCheckBox);
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            List<string> result = new List<string>();
            //Формирование списка выбранных специальностей
            foreach (CheckBox item in this.tableLayoutPanelTop.Controls)
                if (item.Checked) result.Add(item.Text);
            ViewEployee.SetSelectedEmploymentTypes(result);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}