using System;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace WinFormTest
{
    public class MultiSelectEditingHelper
    {
        private GridView view;
        private GridColumn[] columns;
        private bool lockEvents;

        public MultiSelectEditingHelper(GridView view, GridColumn[] changeableColumns)
        {
            this.view = view;
            this.columns = changeableColumns;  
            this.view.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            this.view.MouseUp += view_MouseUp;
            this.view.CellValueChanged += view_CellValueChanged;
            this.view.MouseDown += view_MouseDown;
        }

        private void view_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            OnCellValueChanged(e);
        }

        private void OnCellValueChanged(CellValueChangedEventArgs e)
        {
            if (lockEvents) return;
            lockEvents = true;
            SetSelectedCellsValue(e.Value);
            lockEvents = false;
        }

        private void SetSelectedCellsValue(object value)
        {
            try
            {
                view.BeginUpdate();
                var rows = view.GetSelectedRows();
                foreach (var row in rows) view.SetRowCellValue(row, view.FocusedColumn, value);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                view.EndUpdate();
            }
        }

        private bool GetInSelectedRow(MouseEventArgs e)
        {
            var hi = view.CalcHitInfo(e.Location);
            if (!hi.InRowCell) return false;
            foreach (var column in columns) if (hi.Column == column) return true;
            return false;
        }

        private void view_MouseDown(object sender, MouseEventArgs e)
        {
            if (GetInSelectedRow(e))
            {
                var hi = view.CalcHitInfo(e.Location);
                if (view.FocusedRowHandle == hi.RowHandle)
                {
                    view.FocusedColumn = hi.Column;
                    DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                }
            }
        }

        private void view_MouseUp(object sender, MouseEventArgs e)
        {
            var inSelectedCell = GetInSelectedRow(e);
            if (inSelectedCell)
            {
                DXMouseEventArgs.GetMouseArgs(e).Handled = true;
                view.ShowEditorByMouse();
            }
        }
    }
}
