using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureGridderUtility
{

    public partial class TemplatePropertyPage : UserControl
    {
        protected GridderTemplate _Template = null;
        private FeatureGridderProject _GridderDataset = null;
        private string _Caption = "";

        public virtual string Caption
        {
            get { return _Caption; }
            set { _Caption = value; }
        }


        public TemplatePropertyPage()
        {
            InitializeComponent();
            
        }

        public virtual FeatureGridderProject GridderDataset
        {
            get { return _GridderDataset; }
            set { _GridderDataset = value; }
        }

        public virtual GridderTemplate Template
        {
            get { return _Template; }
            protected set { _Template = value; }
        }

        public virtual void LoadData(GridderTemplate template, FeatureGridderProject dataset)
        {
            Template = template;
        }

        public virtual void UpdateTemplate()
        {
            // to nothing here.
        }

        
    }
}
