// Copyright (c) 2008, 2014, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.Wizards.WindowsForms;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  internal class CSharpEntityFrameworkIndividualControlsStrategy : CSharpIndividualControlsStrategy
  {
    internal CSharpEntityFrameworkIndividualControlsStrategy(StrategyConfig config)
      : base(config)
    {
    }

    protected override void WriteUsingUserCode()
    {
      if (DataAccessTech == DataAccessTechnology.EntityFramework5)
      {
        Writer.WriteLine("using System.Data.Objects;");
      }
      else if (DataAccessTech == DataAccessTechnology.EntityFramework6)
      {
        Writer.WriteLine("using System.Data.Entity.Core.Objects;");
		    Writer.WriteLine("using System.Data.Entity;");
      }
    }

    protected override void WriteFormLoadCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("ctx = new Model1Entities();");
	    if (DataAccessTech == DataAccessTechnology.EntityFramework5)
      {
        Writer.WriteLine("ObjectResult<{0}> _entities = ctx.{0}.Execute(MergeOption.AppendOnly);", CanonicalTableName);
      }
      else if (DataAccessTech == DataAccessTechnology.EntityFramework6)
      {
        Writer.WriteLine("ctx.{0}.Load();", CanonicalTableName);
        Writer.WriteLine("BindingList<{0}> _entities = ctx.{0}.Local.ToBindingList();", CanonicalTableName);
      }
      Writer.WriteLine("{0}BindingSource.DataSource = _entities;", CanonicalTableName);

      for( int i = 0; i < ValidationColumns.Count; i++ )
      {
        ColumnValidation cv = ValidationColumns[i];
        string colName = cv.Name;
        string idColumnCanonical = GetCanonicalIdentifier(colName);
        
        if (cv.HasLookup)
        {
          string canonicalReferencedTableName = GetCanonicalIdentifier(cv.FkInfo.ReferencedTableName);
          Writer.WriteLine("this.{0}_comboBox.DataSource = ctx.{1}.ToList();", idColumnCanonical, canonicalReferencedTableName);
          Writer.WriteLine("this.{0}_comboBox.DisplayMember = \"{1}\";", idColumnCanonical, cv.EfLookupColumnMapping);
          Writer.WriteLine("this.{0}_comboBox.ValueMember = \"{1}\";", idColumnCanonical, cv.FkInfo.ReferencedColumnName);
          Writer.WriteLine("this.{0}_comboBox.DataBindings.Add(new System.Windows.Forms.Binding(\"SelectedValue\", this.{1}BindingSource, \"{2}\", true));",
            idColumnCanonical, CanonicalTableName, cv.EfColumnMapping);
        }
        else if (cv.IsDateType())
        {
          Writer.WriteLine("this.{0}_dateTimePicker.DataBindings.Add(new System.Windows.Forms.Binding(\"Text\", this.{2}BindingSource, \"{1}\", true ));",
            idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
        }
        else if (cv.IsBooleanType())
        {
          Writer.WriteLine("this.{0}CheckBox.DataBindings.Add(new System.Windows.Forms.Binding(\"Checked\", this.{2}BindingSource, \"{1}\", true));",
            idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
        }
        else
        {
          Writer.WriteLine("this.{0}TextBox.DataBindings.Add(new System.Windows.Forms.Binding(\"Text\", this.{2}BindingSource, \"{1}\", true ));",
            idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
        }
      }

      Writer.PopIdentationLevel();
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("private Model1Entities ctx;");

      Writer.PopIdentationLevel();
    }

    protected override void WriteSaveEventCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("{0}BindingSource.EndEdit();", CanonicalTableName);
      Writer.WriteLine("ctx.SaveChanges();");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerControlDeclCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("private System.Windows.Forms.BindingSource {0}BindingSource;", CanonicalTableName);
      for( int i = 0; i < ValidationColumns.Count; i++ )
      {
        ColumnValidation cv = ValidationColumns[i];
        string idColumnCanonical = GetCanonicalIdentifier( cv.Name );
        
        if (cv.HasLookup)
        {
          Writer.WriteLine("private System.Windows.Forms.ComboBox {0}_comboBox;", idColumnCanonical);
        }
        else if (cv.IsDateType())
        {
          Writer.WriteLine("private System.Windows.Forms.DateTimePicker {0}_dateTimePicker;", idColumnCanonical);
        }
        else if ( cv.IsBooleanType() )
        {
          Writer.WriteLine("private System.Windows.Forms.CheckBox {0}CheckBox;", idColumnCanonical);
        }
        else
        {
          Writer.WriteLine("private System.Windows.Forms.TextBox {0}TextBox;", idColumnCanonical);
        }
        Writer.WriteLine("private System.Windows.Forms.Label {0}Label;", idColumnCanonical);
      }
      Writer.WriteLine("private System.Windows.Forms.Panel panel3;");
      Writer.WriteLine("private System.Windows.Forms.Panel panel4;");
      Writer.WriteLine("private System.Windows.Forms.Panel panel5;");
      Writer.WriteLine("private System.Windows.Forms.Panel panel6;");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerControlInitCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("this.bindingNavigator1.BindingSource = this.{0}BindingSource;", CanonicalTableName);
      
      WriteControlInitialization(false);

      // Panel4
      Writer.WriteLine("// ");
      Writer.WriteLine("// panel4");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.panel4.Dock = System.Windows.Forms.DockStyle.Right;");
      Writer.WriteLine("this.panel4.Location = new System.Drawing.Point(656, 0);");
      Writer.WriteLine("this.panel4.Name = \"panel4\";");
      Writer.WriteLine("this.panel4.Size = new System.Drawing.Size(10, 183);");
      Writer.WriteLine("this.panel4.TabIndex = 3;");
      // Panel3
      Writer.WriteLine("// ");
      Writer.WriteLine("// panel3");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.panel3.Controls.Add(this.Panel1);");
      Writer.WriteLine("this.panel3.Controls.Add(this.panel4);");
      Writer.WriteLine("this.panel3.Controls.Add(this.panel5);");
      Writer.WriteLine("this.panel3.Controls.Add(this.panel6);");
      Writer.WriteLine("this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;");
      Writer.WriteLine("this.panel3.Location = new System.Drawing.Point(0, 25);");
      Writer.WriteLine("this.panel3.Name = \"panel3\";");
      Writer.WriteLine("this.panel3.Size = new System.Drawing.Size(666, 183);");
      Writer.WriteLine("this.panel3.TabIndex = 19;");
      // Panel5
      Writer.WriteLine("// ");
      Writer.WriteLine("// panel5");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.panel5.Dock = System.Windows.Forms.DockStyle.Left;");
      Writer.WriteLine("this.panel5.Location = new System.Drawing.Point(0, 0);");
      Writer.WriteLine("this.panel5.Name = \"panel5\";");
      Writer.WriteLine("this.panel5.Size = new System.Drawing.Size(10, 183);");
      Writer.WriteLine("this.panel5.TabIndex = 5;");
      // Panel6
      Writer.WriteLine("// ");
      Writer.WriteLine("// panel6");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;");
      Writer.WriteLine("this.panel6.Location = new System.Drawing.Point(0, 324);");
      Writer.WriteLine("this.panel6.Name = \"panel6\";");
      Writer.WriteLine("this.panel6.Size = new System.Drawing.Size(400, 10);");
      Writer.WriteLine("this.panel6.TabIndex = 6;");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("this.{0}BindingSource = new System.Windows.Forms.BindingSource(this.components);", CanonicalTableName);
      Writer.WriteLine("this.panel3 = new System.Windows.Forms.Panel();");
      Writer.WriteLine("this.panel4 = new System.Windows.Forms.Panel();");
      Writer.WriteLine("this.panel5 = new System.Windows.Forms.Panel();");
      Writer.WriteLine("this.panel6 = new System.Windows.Forms.Panel();");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("this.panel3.SuspendLayout();");
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).BeginInit();", CanonicalTableName);

      Writer.PopIdentationLevel();
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("this.Text = \"{0}\";", CapitalizeString(TableName));
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).EndInit();", CanonicalTableName);
      Writer.WriteLine("this.Controls.Add(this.panel3);");
      Writer.WriteLine("this.panel3.ResumeLayout(false);");
      Writer.WriteLine("this.panel3.PerformLayout();");

      Writer.PopIdentationLevel();
    }
  }
}