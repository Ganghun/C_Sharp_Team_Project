using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace WindowsFormsProject
{ 
    public partial class Form3 : Form
    {
    public Form3()
    {
        InitializeComponent();
    }

    MySqlConnection conn;
    MySqlDataAdapter dataAdapter;
    DataSet dataSet;
    int selectedRowIndex;
     string fsPath="";

        private void Form3_Load(object sender, EventArgs e)
    {
        string connStr = "server=localhost;port=3306;database=C_Sharp;uid=root;pwd=korr3698";
        conn = new MySqlConnection(connStr);
        dataAdapter = new MySqlDataAdapter("SELECT * FROM teacher", conn);
        dataSet = new DataSet();

        onTable.Text = "teacher";
        dataAdapter.Fill(dataSet, "teacher");
        dataGridView1.DataSource = dataSet.Tables["teacher"];
    }

    internal void UpdateRow(string[] rowDatas)
    {
        string sql = "UPDATE teacher SET teacher_no=@제품번호, teacher_name=@제품명, teacher_pno=@재고량, teacher_id=@단가, teacher_pay=@제조업체 WHERE teacher_no=@제품번호";
        dataAdapter.UpdateCommand = new MySqlCommand(sql, conn);
        dataAdapter.UpdateCommand.Parameters.AddWithValue("@제품번호", rowDatas[0]);
        dataAdapter.UpdateCommand.Parameters.AddWithValue("@제품명", rowDatas[1]);
        dataAdapter.UpdateCommand.Parameters.AddWithValue("@재고량", rowDatas[2]);
        dataAdapter.UpdateCommand.Parameters.AddWithValue("@단가", rowDatas[3]);
        dataAdapter.UpdateCommand.Parameters.AddWithValue("@제조업체", rowDatas[4]);
        try
        {
            conn.Open();
            dataAdapter.UpdateCommand.ExecuteNonQuery();

            dataSet.Clear();  // 이전 데이터 지우기
            dataAdapter.Fill(dataSet, "teacher");
            dataGridView1.DataSource = dataSet.Tables["teacher"];
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }

    internal void DeleteRow(string client_id)
    {
        string sql = "DELETE FROM teacher WHERE teacher_no=@제품번호";
        dataAdapter.DeleteCommand = new MySqlCommand(sql, conn);
        dataAdapter.DeleteCommand.Parameters.AddWithValue("@제품번호", client_id);

        try
        {
            conn.Open();
            dataAdapter.DeleteCommand.ExecuteNonQuery();

            dataSet.Clear();
            dataAdapter.Fill(dataSet, "teacher");
            dataGridView1.DataSource = dataSet.Tables["teacher"];
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }

    private void btn_select_Click(object sender, EventArgs e)
    {
        string sql = "SELECT * FROM teacher WHERE ";

        #region 검색 조건 설정
        string[] conditions = new string[5];
        conditions[0] = (tbID.Text != "") ? "teacher_no = @제품번호" : null;
        conditions[1] = (tbName.Text != "") ? "teacher_name = @제품명" : null;
        conditions[2] = (tbAge.Text != "") ? "teacher_pno= @재고량" : null;
        conditions[3] = (tbRank.Text != "") ? "teacher_id = @단가" : null;
        conditions[4] = (tbJob.Text != "") ? "teacher_pay = @제조업체" : null;

        if (tbID.Text != "" || tbAge.Text != "" || tbName.Text != "" || tbRank.Text != "" || tbJob.Text != "")
        {
            bool isFirst = true;
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i] != null)
                {
                    if (isFirst)
                    {
                        sql += conditions[i];
                    }
                    else
                    {
                        sql += " and " + conditions[i];
                    }
                }
            }
        }
        else
        {
            sql = "SELECT * FROM teacher";
        }
        #endregion

        dataAdapter.SelectCommand = new MySqlCommand(sql, conn);
        dataAdapter.SelectCommand.Parameters.AddWithValue("@제품번호", tbID.Text);
        dataAdapter.SelectCommand.Parameters.AddWithValue("@제품명", tbName.Text);
        dataAdapter.SelectCommand.Parameters.AddWithValue("@재고량", tbAge.Text);
        dataAdapter.SelectCommand.Parameters.AddWithValue("@단가", tbRank.Text);
        dataAdapter.SelectCommand.Parameters.AddWithValue("@제조업체", tbJob.Text);

        try
        {
            conn.Open();
            dataSet.Clear();
            if (dataAdapter.Fill(dataSet, "teacher") > 0)
                dataGridView1.DataSource = dataSet.Tables["teacher"];
            else
                MessageBox.Show("찾는 데이터가 없습니다.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            conn.Close();
        }

    }

    private void btn_insert_Click(object sender, EventArgs e)
    {
        string sql = "INSERT INTO teacher(teacher_no,teacher_name,teacher_pno,teacher_id,teacher_pay)" +
            " VALUES(";

        string[] conditions = new string[5];
        conditions[0] = (tbID.Text != "") ? "@제품번호" : null;
        conditions[1] = (tbName.Text != "") ? "@제품명" : null;
        conditions[2] = (tbAge.Text != "") ? "@재고량" : null;
        conditions[3] = (tbRank.Text != "") ? "@단가" : null;
        conditions[4] = (tbJob.Text != "") ? "@제조업체" : null;

        if (tbID.Text != "" && tbAge.Text != "" && tbName.Text != "" && tbRank.Text != "" && tbJob.Text != "")
        {
            bool isFirst = true;
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i] != null)
                {
                    if (isFirst)
                    {
                        sql += conditions[i];
                        isFirst = false;
                    }
                    else if (i == conditions.Length - 1)
                    {
                        sql += "," + conditions[i] + ")";
                    }
                    else
                    {
                        sql += ", " + conditions[i];
                    }
                }
            }
        }
        else
        {
            MessageBox.Show("모든 정보를 입력하세요");
        }


        testcode.Text = sql;

        dataAdapter.InsertCommand = new MySqlCommand(sql, conn);
        dataAdapter.InsertCommand.Parameters.AddWithValue("@제품번호", tbID.Text);
        dataAdapter.InsertCommand.Parameters.AddWithValue("@제품명", tbName.Text);
        dataAdapter.InsertCommand.Parameters.AddWithValue("@재고량", tbAge.Text);
        dataAdapter.InsertCommand.Parameters.AddWithValue("@단가", tbRank.Text);
        dataAdapter.InsertCommand.Parameters.AddWithValue("@제조업체", tbJob.Text);

        try
        {
            conn.Open();
            dataAdapter.InsertCommand.ExecuteNonQuery();

            dataSet.Clear();                                        // 이전 데이터 지우기
            dataAdapter.Fill(dataSet, "teacher");                      // DB -> DataSet
            dataGridView1.DataSource = dataSet.Tables["teacher"];      // dataGridView에 테이블 표시                                     // 텍스트 박스 내용 지우기
        }
        catch (Exception)
        {
            //MessageBox.Show(ex.Message);
        }
        finally
        {
            conn.Close();
        }


    }

    private void cleaner_Click(object sender, EventArgs e)
    {
        tbID.Clear();
        tbName.Clear();
        tbAge.Clear();
        tbRank.Clear();
        tbJob.Clear();
    }

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        selectedRowIndex = e.RowIndex;
        DataGridViewRow row = dataGridView1.Rows[selectedRowIndex];

        // 새로운 폼에 선택된 row의 정보를 담아서 생성
        Form4 Dig = new Form4(
            selectedRowIndex,
            row.Cells[0].Value.ToString(),
            row.Cells[1].Value.ToString(),
            row.Cells[2].Value.ToString(),
            row.Cells[3].Value.ToString(),
            row.Cells[4].Value.ToString()
            );

        Dig.Owner = this;               // 새로운 폼의 부모가 Form1 인스턴스임을 지정
        Dig.ShowDialog();               // 폼 띄우기(Modal)
        Dig.Dispose();
    }

        private void TextFileSave()
        {

            // SaveFileDialog에 지정한 파일경로에 Stream 생성
            using (StreamWriter sw = new StreamWriter(fsPath))
            {
                // Column Title 한번 출력
                foreach (DataColumn col in dataSet.Tables["teacher"].Columns)
                {
                    sw.Write($"{col.ColumnName}\t");
                }
                sw.WriteLine();

                // DataGridView에 기록된 모든 파일 정보를 Text파일에 출력
                foreach (DataRow row in dataSet.Tables["teacher"].Rows)
                {
                    string rowString = "";
                    foreach (var item in row.ItemArray)
                    {
                        rowString += item.ToString() + "\t";
                    }
                    sw.WriteLine(rowString);
                }
                sw.Close();
            }
        }


        private void FormAdapter_Click(object sender, EventArgs e)
        {
            Form5 Dig = new Form5();

            Dig.Owner = this;               // 새로운 폼의 부모가 Form1 인스턴스임을 지정
            Dig.ShowDialog();               // 폼 띄우기(Modal)
            Dig.Dispose();
        }


        private void button1_Click(object sender, EventArgs e)
        {
                this.saveFileDialog1.FileName = "TempName";
                this.saveFileDialog1.DefaultExt = "xls";
                this.saveFileDialog1.Filter = "Excel files (*.xls)|*.xls";
                this.saveFileDialog1.InitialDirectory = "C:\\Users\\User\\Desktop";

                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    int num = 0;
                    object missingType = Type.Missing;

                    Excel.Application objApp;
                    Excel._Workbook objBook;
                    Excel.Workbooks objBooks;
                    Excel.Sheets objSheets;
                    Excel._Worksheet objSheet;
                    Excel.Range range;

                    string[] headers = new string[dataGridView1.ColumnCount];
                    string[] columns = new string[dataGridView1.ColumnCount];

                    for (int c = 0; c < dataGridView1.ColumnCount; c++)
                    {
                        headers[c] = dataGridView1.Rows[0].Cells[c].OwningColumn.HeaderText.ToString();
                        num = c + 65;
                        columns[c] = Convert.ToString((char)num);
                    }

                    try
                    {
                        objApp = new Excel.Application();
                        objBooks = objApp.Workbooks;
                        objBook = objBooks.Add(Missing.Value);
                        objSheets = objBook.Worksheets;
                        objSheet = (Excel._Worksheet)objSheets.get_Item(1);

                        if (true)
                        {
                            for (int c = 0; c < dataGridView1.ColumnCount; c++)
                            {
                                range = objSheet.get_Range(columns[c] + "1", Missing.Value);
                                range.set_Value(Missing.Value, headers[c]);
                            }
                        }

                        for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                        {
                            for (int j = 0; j < dataGridView1.ColumnCount; j++)
                            {
                                range = objSheet.get_Range(columns[j] + Convert.ToString(i + 2),
                                                                       Missing.Value);
                                range.set_Value(Missing.Value,
                                                      dataGridView1.Rows[i].Cells[j].Value.ToString());
                            }
                        }

                        objApp.Visible = false;
                        objApp.UserControl = false;

                        objBook.SaveAs(@saveFileDialog1.FileName,
                                  Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                                  missingType, missingType, missingType, missingType,
                                  Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                                  missingType, missingType, missingType, missingType, missingType);
                        objBook.Close(false, missingType, missingType);

                        Cursor.Current = Cursors.Default;

                        MessageBox.Show("Save Success!!!");
                    }
                    catch (Exception theException)
                    {
                        String errorMessage;
                        errorMessage = "Error: ";
                        errorMessage = String.Concat(errorMessage, theException.Message);
                        errorMessage = String.Concat(errorMessage, " Line: ");
                        errorMessage = String.Concat(errorMessage, theException.Source);

                        MessageBox.Show(errorMessage, "Error");
                    }
                }
        }

        private void txt_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "텍스트 파일(*.txt)|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fsPath = saveFileDialog1.FileName;  // SaveFileDialog에 지정한 파일경로를 전역 변수인 fsPath에 저장
                TextFileSave();
            }
        }
    }
}
