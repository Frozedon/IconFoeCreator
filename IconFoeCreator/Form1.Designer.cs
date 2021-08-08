
namespace IconFoeCreator
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_Faction = new System.Windows.Forms.Label();
            this.comboBox_Faction = new System.Windows.Forms.ComboBox();
            this.comboBox_Job = new System.Windows.Forms.ComboBox();
            this.label_Job = new System.Windows.Forms.Label();
            this.comboBox_Chapter = new System.Windows.Forms.ComboBox();
            this.label_Chapter = new System.Windows.Forms.Label();
            this.richTextBox_Description = new System.Windows.Forms.RichTextBox();
            this.checkBox_Damage = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label_Faction
            // 
            this.label_Faction.AutoSize = true;
            this.label_Faction.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Faction.Location = new System.Drawing.Point(43, 51);
            this.label_Faction.Name = "label_Faction";
            this.label_Faction.Size = new System.Drawing.Size(123, 37);
            this.label_Faction.TabIndex = 0;
            this.label_Faction.Text = "Faction";
            // 
            // comboBox_Faction
            // 
            this.comboBox_Faction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Faction.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_Faction.FormattingEnabled = true;
            this.comboBox_Faction.Location = new System.Drawing.Point(50, 91);
            this.comboBox_Faction.Name = "comboBox_Faction";
            this.comboBox_Faction.Size = new System.Drawing.Size(394, 45);
            this.comboBox_Faction.TabIndex = 1;
            // 
            // comboBox_Job
            // 
            this.comboBox_Job.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Job.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_Job.FormattingEnabled = true;
            this.comboBox_Job.Location = new System.Drawing.Point(50, 194);
            this.comboBox_Job.Name = "comboBox_Job";
            this.comboBox_Job.Size = new System.Drawing.Size(394, 45);
            this.comboBox_Job.TabIndex = 3;
            // 
            // label_Job
            // 
            this.label_Job.AutoSize = true;
            this.label_Job.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Job.Location = new System.Drawing.Point(43, 154);
            this.label_Job.Name = "label_Job";
            this.label_Job.Size = new System.Drawing.Size(69, 37);
            this.label_Job.TabIndex = 2;
            this.label_Job.Text = "Job";
            // 
            // comboBox_Chapter
            // 
            this.comboBox_Chapter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Chapter.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_Chapter.FormattingEnabled = true;
            this.comboBox_Chapter.Location = new System.Drawing.Point(50, 304);
            this.comboBox_Chapter.Name = "comboBox_Chapter";
            this.comboBox_Chapter.Size = new System.Drawing.Size(77, 45);
            this.comboBox_Chapter.TabIndex = 5;
            // 
            // label_Chapter
            // 
            this.label_Chapter.AutoSize = true;
            this.label_Chapter.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Chapter.Location = new System.Drawing.Point(43, 264);
            this.label_Chapter.Name = "label_Chapter";
            this.label_Chapter.Size = new System.Drawing.Size(131, 37);
            this.label_Chapter.TabIndex = 4;
            this.label_Chapter.Text = "Chapter";
            // 
            // richTextBox_Description
            // 
            this.richTextBox_Description.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_Description.Location = new System.Drawing.Point(490, 51);
            this.richTextBox_Description.Name = "richTextBox_Description";
            this.richTextBox_Description.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox_Description.Size = new System.Drawing.Size(643, 843);
            this.richTextBox_Description.TabIndex = 7;
            this.richTextBox_Description.Text = "";
            // 
            // checkBox_Damage
            // 
            this.checkBox_Damage.AutoSize = true;
            this.checkBox_Damage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_Damage.Location = new System.Drawing.Point(50, 840);
            this.checkBox_Damage.Name = "checkBox_Damage";
            this.checkBox_Damage.Size = new System.Drawing.Size(260, 36);
            this.checkBox_Damage.TabIndex = 8;
            this.checkBox_Damage.Text = "Use Flat Damage";
            this.checkBox_Damage.UseVisualStyleBackColor = true;
            this.checkBox_Damage.CheckedChanged += new System.EventHandler(this.checkBox_Damage_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 919);
            this.Controls.Add(this.checkBox_Damage);
            this.Controls.Add(this.richTextBox_Description);
            this.Controls.Add(this.comboBox_Chapter);
            this.Controls.Add(this.label_Chapter);
            this.Controls.Add(this.comboBox_Job);
            this.Controls.Add(this.label_Job);
            this.Controls.Add(this.comboBox_Faction);
            this.Controls.Add(this.label_Faction);
            this.Name = "Form1";
            this.Text = "Icon Foe Creator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Faction;
        private System.Windows.Forms.ComboBox comboBox_Faction;
        private System.Windows.Forms.ComboBox comboBox_Job;
        private System.Windows.Forms.Label label_Job;
        private System.Windows.Forms.ComboBox comboBox_Chapter;
        private System.Windows.Forms.Label label_Chapter;
        private System.Windows.Forms.RichTextBox richTextBox_Description;
        private System.Windows.Forms.CheckBox checkBox_Damage;
    }
}

