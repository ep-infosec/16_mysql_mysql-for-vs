/*
 * ++++++++++++++++++++++++++++++++++++++++++++++++++
 * This code is generated by a tool and is provided "as is", without warranty of any kind,
 * express or implied, including but not limited to the warranties of merchantability,
 * fitness for a particular purpose and non-infringement.
 * In no event shall the authors or copyright holders be liable for any claim, damages or
 * other liability, whether in an action of contract, tort or otherwise, arising from,
 * out of or in connection with the software or the use or other dealings in the software.
 * ++++++++++++++++++++++++++++++++++++++++++++++++++
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace $ProjectNamespace$
{
  public class MyDateTimePickerColumn : DataGridViewColumn
  {
    public MyDateTimePickerColumn()
      : base(new MyDateTimePickerCell())
    {
    }

    public override DataGridViewCell CellTemplate
    {
      get { return base.CellTemplate; }
      set
      {
        if (value != null && !value.GetType().IsAssignableFrom(typeof(MyDateTimePickerCell)))
        {
          throw new InvalidCastException("Type must be MyDateTimePickerCell");
        }
        base.CellTemplate = value;
      }
    }
  }

  public class MyDateTimePickerCell : DataGridViewTextBoxCell
  {
    public static readonly string DATE_FORMAT = "dd/MM/yyyy, hh:mm";

    public MyDateTimePickerCell()
      : base()
    {
      this.Style.Format = DATE_FORMAT;
    }

    public override void InitializeEditingControl(int rowIndex, object
        initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
      base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

      MyDateTimePickerEditingControl editCtrl = DataGridView.EditingControl as MyDateTimePickerEditingControl;
      if (Value == null) { editCtrl.Value = (DateTime)DefaultNewRowValue; }
      else { editCtrl.Value = (DateTime)Value; }
    }

    public override Type EditType
    {
      get { return typeof(MyDateTimePickerEditingControl); }
    }

    public override Type ValueType
    {
      get { return typeof(System.DateTime); }
    }

    public override object DefaultNewRowValue
    {
      get { return DateTime.Today; }
    }
  }

  public class MyDateTimePickerEditingControl : DateTimePicker, IDataGridViewEditingControl
  {
    private DataGridView _dataGridView;
    private bool _valueChanged = false;
    private int _rowIndex;

    public MyDateTimePickerEditingControl()
    {
      this.Format = DateTimePickerFormat.Custom;
      this.CustomFormat = MyDateTimePickerCell.DATE_FORMAT;
    }

    public object EditingControlFormattedValue
    {
      get
      {
        return Value.ToString(MyDateTimePickerCell.DATE_FORMAT);
      }
      set
      {
        if (value is string)
        {
          DateTime _value;
          if (!DateTime.TryParse((string)value, out _value))
          {
            Value = DateTime.Today;
          }
          else
          {
            Value = _value;
          }
        }
      }
    }

    public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
    {
      return EditingControlFormattedValue;
    }

    public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
    {
      Font = dataGridViewCellStyle.Font;
      CalendarForeColor = dataGridViewCellStyle.ForeColor;
      CalendarMonthBackground = dataGridViewCellStyle.BackColor;
      CalendarTitleBackColor = dataGridViewCellStyle.BackColor;
    }

    public int EditingControlRowIndex
    {
      get { return _rowIndex; }
      set { _rowIndex = value; }
    }

    public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
    {
      switch (key & Keys.KeyCode)
      {
        case Keys.Left:
        case Keys.Up:
        case Keys.Down:
        case Keys.Right:
        case Keys.Home:
        case Keys.End:
        case Keys.PageDown:
        case Keys.PageUp:
          return true;
        default:
          return !dataGridViewWantsInputKey;
      }
    }

    public void PrepareEditingControlForEdit(bool selectAll)
    {
      // nothing
    }

    public bool RepositionEditingControlOnValueChange
    {
      get { return false; }
    }

    public DataGridView EditingControlDataGridView
    {
      get { return _dataGridView; }
      set { _dataGridView = value; }
    }

    public bool EditingControlValueChanged
    {
      get { return _valueChanged; }
      set { _valueChanged = value; }
    }

    public Cursor EditingPanelCursor { get { return base.Cursor; } }

    protected override void OnValueChanged(EventArgs e)
    {
      _valueChanged = true;
      this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
      base.OnValueChanged(e);
    }
  }
}
