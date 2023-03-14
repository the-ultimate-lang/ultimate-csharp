using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace WinForm6_0.WinExe
{
    public partial class Form1 : Form
    {
        string clickedNode;
        /*
           private void InitializeComponent2()
           {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.listView1 = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer1
            //
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            //
            // splitContainer1.Panel1
            //
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            //
            // splitContainer1.Panel2
            //
            this.splitContainer1.Panel2.Controls.Add(this.listView1);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 1;
            //
            // treeView1
            //
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(266, 450);
            this.treeView1.TabIndex = 0;
            //
            // listView1
            //
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(530, 450);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

           }
         */
        public Form1()
        {
            InitializeComponent();
#if NET472
            this.Text = "net472";
#endif
            Console.WriteLine("In Form1()");
            //���[�U�[���T�C�Y��ύX�ł��Ȃ��悤�ɂ���
            //�ő剻�A�ŏ����͂ł���
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //�ő�T�C�Y�ƍŏ��T�C�Y�����݂̃T�C�Y�ɐݒ肷��
            //this.MaximumSize = this.Size;
            //this.MinimumSize = this.Size;
            var drives = Environment.GetLogicalDrives();
            Console.WriteLine(drives);
            /// <summary>
            /// �N�����̏���
            /// </summary>
            // �h���C�u�ꗗ�𑖍����ăc���[�ɒǉ�
            foreach (String drive in Environment.GetLogicalDrives())
            {
                // �V�K�m�[�h�쐬
                // �v���X�{�^����\�����邽�ߋ�̃m�[�h��ǉ����Ă���
                TreeNode node = new TreeNode(drive);
                node.Nodes.Add(new TreeNode());
                treeView1.Nodes.Add(node);
            }
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Clear();
            listView1.Columns.Add("���O");
            listView1.Columns.Add("�X�V����");
            listView1.Columns.Add("�T�C�Y");
            /*
               var columnName = new ColumnHeader();
               var columnTime = new ColumnHeader();
               var columnSize = new ColumnHeader();
               columnName.Text = "���O";
               columnName.Width = 100;
               columnTime.Text = "�X�V����";
               columnTime.Width = 150;
               columnSize.Text = "�T�C�Y";
               columnSize.Width = 150;
               ColumnHeader[] colHeaderRegValue = { columnName, columnTime, columnSize };
               listView1.Columns.AddRange(colHeaderRegValue);
             */
            // �����I���h���C�u�̓��e��\��
            setListItem(Environment.GetLogicalDrives().First());
        }
        private void setListItem(String filePath)
        {
            //MessageBox.Show("setListItem(): " + filePath);
            Console.WriteLine("setListItem(): " + filePath);
            listView1.BeginUpdate();
            listView1.Items.Clear();
            try
            {
                /*
                   // �t�H���_�ꗗ
                   DirectoryInfo dirList = new DirectoryInfo(filePath);
                   foreach (DirectoryInfo di in dirList.GetDirectories())
                   {
                    //Console.WriteLine("setListItem(di.Name): " + di.Name);
                    ListViewItem item = new ListViewItem(di.Name);
                    item.SubItems.Add(String.Format("{0:yyyy/MM/dd HH:mm:ss}", di.LastAccessTime));
                    item.SubItems.Add("");
                    listView1.Items.Add(item);
                   }
                 */
                // �t�@�C���ꗗ
                List<String> files = Directory.GetFiles(filePath).ToList<String>();
                foreach (String file in files)
                {
                    FileInfo info = new FileInfo(file);
                    //Console.WriteLine("setListItem(di.Name): " + info.Name);
                    ListViewItem item = new ListViewItem(info.Name);
                    item.SubItems.Add(String.Format("{0:yyyy/MM/dd HH:mm:ss}", info.LastAccessTime));
                    item.SubItems.Add(getFileSize(info.Length));
                    listView1.Items.Add(item);
                    //listView1.Items.Add(new ListViewItem(new string[] { info.Name, String.Format("{0:yyyy/MM/dd HH:mm:ss}", info.LastAccessTime), getFileSize(info.Length) }));
                    //listBox1.Items.Add(info.Name);
                }
            }
            catch (IOException ie)
            {
                //MessageBox.Show(ie.Message, "�I���G���[");
            }
            // �񕝂���������
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.EndUpdate();
            //listView1.Refresh();
            //this.Refresh();
            //Console.WriteLine(listBox1.Items.Count);
        }
        private String getFileSize(long fileSize)
        {
            String ret = fileSize + " �o�C�g";
            if (fileSize > (1024f * 1024f * 1024f))
            {
                ret = Math.Round((fileSize / 1024f / 1024f / 1024f), 2).ToString() + " GB";
            }
            else if (fileSize > (1024f * 1024f))
            {
                ret = Math.Round((fileSize / 1024f / 1024f), 2).ToString() + " MB";
            }
            else if (fileSize > 1024f)
            {
                ret = Math.Round((fileSize / 1024f)).ToString() + " KB";
            }
            return ret;
        }
        void myMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new Form();
            frm.Text = clickedNode;
            frm.ShowDialog(this);
            clickedNode = "";
        }
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            String path = node.FullPath;
            node.Nodes.Clear();
            try
            {
                DirectoryInfo dirList = new DirectoryInfo(path);
                foreach (DirectoryInfo di in dirList.GetDirectories())
                {
                    TreeNode child = new TreeNode(di.Name);
                    child.Nodes.Add(new TreeNode());
                    node.Nodes.Add(child);
                }
            }
            catch (IOException ie)
            {
                //MessageBox.Show(ie.Message, "�I���G���[");
            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Console.WriteLine("treeView1_NodeMouseClick(): " + e.Node.FullPath);
            //treeView1.SelectedNode.IsSelected = true;
            treeView1.SelectedNode = e.Node;
            setListItem(e.Node.FullPath);
            if (e.Button == MouseButtons.Right)
            {
#if !NET5_0_OR_GREATER
                clickedNode = e.Node.FullPath;
                ContextMenu mnu = new ContextMenu();
                MenuItem myMenuItem = new MenuItem("Show Me: " + e.Node.FullPath);
                myMenuItem.Click += new EventHandler(myMenuItem_Click);
                mnu.MenuItems.Add(myMenuItem);
                mnu.Show(treeView1, e.Location);
#endif
            }
        }
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = listView1.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
#if !NET5_0_OR_GREATER
                    ContextMenu mnu = new ContextMenu();
                    MenuItem myMenuItem = new MenuItem("Show Me: " + focusedItem.Text);
                    //myMenuItem.Click += new EventHandler(myMenuItem_Click);
                    mnu.MenuItems.Add(myMenuItem);
                    mnu.Show(listView1, e.Location);
#endif
                }
            }
        }
    }
}