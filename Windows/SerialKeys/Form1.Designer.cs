namespace SerialKeys
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
            if (disposing && (components != null)) {
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
            this.label1 = new System.Windows.Forms.Label();
            this.portList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.baudList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.vk0 = new System.Windows.Forms.TextBox();
            this.vk1 = new System.Windows.Forms.TextBox();
            this.vk3 = new System.Windows.Forms.TextBox();
            this.vk2 = new System.Windows.Forms.TextBox();
            this.vk7 = new System.Windows.Forms.TextBox();
            this.vk6 = new System.Windows.Forms.TextBox();
            this.vk5 = new System.Windows.Forms.TextBox();
            this.vk4 = new System.Windows.Forms.TextBox();
            this.vk10 = new System.Windows.Forms.TextBox();
            this.vk9 = new System.Windows.Forms.TextBox();
            this.vk8 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // portList
            // 
            this.portList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.portList.FormattingEnabled = true;
            this.portList.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12"});
            this.portList.Location = new System.Drawing.Point(48, 10);
            this.portList.Name = "portList";
            this.portList.Size = new System.Drawing.Size(68, 21);
            this.portList.TabIndex = 1;
            this.portList.SelectedValueChanged += new System.EventHandler(this.portList_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(122, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Baud:";
            // 
            // baudList
            // 
            this.baudList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baudList.FormattingEnabled = true;
            this.baudList.Items.AddRange(new object[] {
            "300",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.baudList.Location = new System.Drawing.Point(165, 11);
            this.baudList.Name = "baudList";
            this.baudList.Size = new System.Drawing.Size(71, 21);
            this.baudList.TabIndex = 3;
            this.baudList.SelectedValueChanged += new System.EventHandler(this.baudList_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(103, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Virt Keycode";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(293, 10);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 40;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "Input 0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Input 1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Input 2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 186);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 44;
            this.label7.Text = "Input 3";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 290);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 13);
            this.label8.TabIndex = 48;
            this.label8.Text = "Input 7";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(34, 264);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 13);
            this.label9.TabIndex = 47;
            this.label9.Text = "Input 6";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(34, 238);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 46;
            this.label10.Text = "Input 5";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(34, 212);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(40, 13);
            this.label11.TabIndex = 45;
            this.label11.Text = "Input 4";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(34, 368);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 13);
            this.label12.TabIndex = 51;
            this.label12.Text = "Input 10";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(34, 342);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(40, 13);
            this.label13.TabIndex = 50;
            this.label13.Text = "Input 9";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(34, 316);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(40, 13);
            this.label14.TabIndex = 49;
            this.label14.Text = "Input 8";
            // 
            // vk0
            // 
            this.vk0.Location = new System.Drawing.Point(106, 105);
            this.vk0.Name = "vk0";
            this.vk0.Size = new System.Drawing.Size(63, 20);
            this.vk0.TabIndex = 52;
            this.vk0.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk1
            // 
            this.vk1.Location = new System.Drawing.Point(106, 131);
            this.vk1.Name = "vk1";
            this.vk1.Size = new System.Drawing.Size(63, 20);
            this.vk1.TabIndex = 53;
            this.vk1.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk3
            // 
            this.vk3.Location = new System.Drawing.Point(106, 183);
            this.vk3.Name = "vk3";
            this.vk3.Size = new System.Drawing.Size(63, 20);
            this.vk3.TabIndex = 55;
            this.vk3.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk2
            // 
            this.vk2.Location = new System.Drawing.Point(106, 157);
            this.vk2.Name = "vk2";
            this.vk2.Size = new System.Drawing.Size(63, 20);
            this.vk2.TabIndex = 54;
            this.vk2.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk7
            // 
            this.vk7.Location = new System.Drawing.Point(106, 287);
            this.vk7.Name = "vk7";
            this.vk7.Size = new System.Drawing.Size(63, 20);
            this.vk7.TabIndex = 59;
            this.vk7.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk6
            // 
            this.vk6.Location = new System.Drawing.Point(106, 261);
            this.vk6.Name = "vk6";
            this.vk6.Size = new System.Drawing.Size(63, 20);
            this.vk6.TabIndex = 58;
            this.vk6.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk5
            // 
            this.vk5.Location = new System.Drawing.Point(106, 235);
            this.vk5.Name = "vk5";
            this.vk5.Size = new System.Drawing.Size(63, 20);
            this.vk5.TabIndex = 57;
            this.vk5.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk4
            // 
            this.vk4.Location = new System.Drawing.Point(106, 209);
            this.vk4.Name = "vk4";
            this.vk4.Size = new System.Drawing.Size(63, 20);
            this.vk4.TabIndex = 56;
            this.vk4.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk10
            // 
            this.vk10.Location = new System.Drawing.Point(106, 365);
            this.vk10.Name = "vk10";
            this.vk10.Size = new System.Drawing.Size(63, 20);
            this.vk10.TabIndex = 62;
            this.vk10.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk9
            // 
            this.vk9.Location = new System.Drawing.Point(106, 339);
            this.vk9.Name = "vk9";
            this.vk9.Size = new System.Drawing.Size(63, 20);
            this.vk9.TabIndex = 61;
            this.vk9.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // vk8
            // 
            this.vk8.Location = new System.Drawing.Point(106, 313);
            this.vk8.Name = "vk8";
            this.vk8.Size = new System.Drawing.Size(63, 20);
            this.vk8.TabIndex = 60;
            this.vk8.TextChanged += new System.EventHandler(this.vk0_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 413);
            this.Controls.Add(this.vk10);
            this.Controls.Add(this.vk9);
            this.Controls.Add(this.vk8);
            this.Controls.Add(this.vk7);
            this.Controls.Add(this.vk6);
            this.Controls.Add(this.vk5);
            this.Controls.Add(this.vk4);
            this.Controls.Add(this.vk3);
            this.Controls.Add(this.vk2);
            this.Controls.Add(this.vk1);
            this.Controls.Add(this.vk0);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.baudList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.portList);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Serial Port Keystroke Sender";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox portList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox baudList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox vk0;
        private System.Windows.Forms.TextBox vk1;
        private System.Windows.Forms.TextBox vk3;
        private System.Windows.Forms.TextBox vk2;
        private System.Windows.Forms.TextBox vk7;
        private System.Windows.Forms.TextBox vk6;
        private System.Windows.Forms.TextBox vk5;
        private System.Windows.Forms.TextBox vk4;
        private System.Windows.Forms.TextBox vk10;
        private System.Windows.Forms.TextBox vk9;
        private System.Windows.Forms.TextBox vk8;
    }
}

