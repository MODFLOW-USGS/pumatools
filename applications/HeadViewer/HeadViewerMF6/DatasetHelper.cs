using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;

namespace HeadViewerMF6
{
    public class DatasetHelper
    {
        public static void InitializeDatasetTreeview(System.Windows.Forms.TreeView tree)
        {
            if (tree == null)
                throw new ArgumentNullException("tree");

            // Suspend repainting the treeview
            tree.BeginUpdate();

            // Clear the treeview
            tree.Nodes.Clear();


            TreeNode node = null;
            TreeNode node2 = null;
            node = tree.Nodes.Add("data", "MODFLOW Layer Output");
            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;
            //node2 = node.Nodes.Add("dataset", "Dataset: <none>");
            //node2.ImageIndex = 2;
            //node2.SelectedImageIndex = 2;
            node2 = node.Nodes.Add("files", "Head and Drawdown Files");
            node2.ImageIndex = 2;
            node2.SelectedImageIndex = 2;
            node.Expand();


            // Begin repainting the treeview
            tree.EndUpdate();

        }
        public static TreeNode AddDataset(TreeView tree, DatasetInfo dataset)
        {            
            TreeNode node = null;
            TreeNode node2 = null;

            string key = dataset.DatasetNameFile.ToLower();
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The specified pathname is empty.");

            TreeNode dataRoot = tree.Nodes["data"];

            int index = dataRoot.Nodes.IndexOfKey("dataset");
            if (index > -1)
            { dataRoot.Nodes.RemoveByKey("dataset"); }
            TreeNode datasetNode = dataRoot.Nodes.Insert(0, "dataset", "Dataset " + dataset.DatasetBaseName);
            
            datasetNode.ToolTipText = dataset.DatasetNameFile;
            datasetNode.ImageIndex = 2;
            datasetNode.SelectedImageIndex = 2;
            DataItemTag tag = new DataItemTag();
            tag.DatasetKey = key;
            tag.IsDatasetNode = true;
            datasetNode.Tag = tag;

            if (!string.IsNullOrEmpty(dataset.BinaryHeadFile.ToLower()))
            {
                node2 = datasetNode.Nodes.Add("head", "Head");
                node2.Text = "Head (" + System.IO.Path.GetFileName(dataset.BinaryHeadFile) + ")";
                tag = new DataItemTag(key, true, dataset.BinaryHeadFile, "HEAD");
                tag.Label = node2.Parent.Text + " -- " + node2.Text;
                tag.HNoFlo = dataset.HNoFlo;
                tag.HDry = dataset.HDry;
                node2.Tag = tag;
                node2.ToolTipText = dataset.BinaryHeadFile;
                node2.EnsureVisible();
                node2.ImageIndex = 1;
                node2.SelectedImageIndex = 1;
            }
            if (!string.IsNullOrEmpty(dataset.BinaryDrawdownFile.ToLower()))
            {
                node2 = datasetNode.Nodes.Add("drawdown", "Drawdown");
                node2.Text = "Drawdown (" + System.IO.Path.GetFileName(dataset.BinaryDrawdownFile) + ")";
                tag = new DataItemTag(key, true, dataset.BinaryDrawdownFile, "DRAWDOWN");
                tag.Label = node2.Parent.Text + " -- " + node2.Text;
                tag.HNoFlo = dataset.HNoFlo;
                tag.HDry = dataset.HDry;
                node2.Tag = tag;
                node2.ToolTipText = dataset.BinaryDrawdownFile;
                node2.EnsureVisible();
                node2.ImageIndex = 1;
                node2.SelectedImageIndex = 1;
            }
            datasetNode.Expand();

            return datasetNode;

        }
        public static TreeNode AddFile(TreeView tree, string filename, Collection<string> dataTypes, float hNoFlo, float hDry)
        {
            TreeNode node = null;
            TreeNode node2 = null;
            TreeNode filesRoot = tree.Nodes["data"].Nodes["files"];

            string name = filename.Trim();
            string key = name.ToLower();
            if (string.IsNullOrEmpty(key))
            { throw new ArgumentNullException("The filename is empty."); }

            if (filesRoot.Nodes.ContainsKey(key))
            {
                MessageBox.Show("The file already exists in this project.");
            }
            else
            {
                node = filesRoot.Nodes.Add(key, name);
                DataItemTag tag = new DataItemTag(key, true, name, dataTypes[0]);
                char[] delimiters = new char[1];
                delimiters[0] = System.IO.Path.DirectorySeparatorChar;
                string[] tokens = name.Split(delimiters);
                int count = tokens.Length;
                string namePart2 = tokens[count - 1];
                string namePart1 = tokens[count - 2];
                string shortName = System.IO.Path.Combine(namePart1, namePart2);
                node.Text = System.IO.Path.Combine("...", shortName);
                //node.Text = System.IO.Path.GetFileName(name);
                tag.Label = node.Text;
                tag.IsFileNode = true;
                tag.IsDatasetNode = false;
                tag.HNoFlo = hNoFlo;
                tag.HDry = hDry;
                node.ToolTipText = name;
                node.EnsureVisible();
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;
                node.Tag = tag;
            }

            return node;

        }

        public static TreeNode GetDrawdownNode(TreeView tree)
        {
            TreeNode dataset = tree.Nodes["data"].Nodes["dataset"];
            if (dataset.Nodes.ContainsKey("drawdown"))
            {
                return dataset.Nodes["drawdown"];
            }
            else
            { return null; }

        }

        public static TreeNode GetHeadNode(TreeView tree)
        {
            TreeNode dataset = tree.Nodes["data"].Nodes["dataset"];
            if (dataset.Nodes.ContainsKey("head"))
            {
                return dataset.Nodes["head"];
            }
            else
            { return null; }

        }

        //public static void RemoveData(TreeView tree, string key)
        //{
        //    TreeNode node = null;
        //    int index = tree.Nodes.IndexOfKey("datasets");
        //    if (index > -1)
        //    {
        //        node = tree.Nodes[index];
        //        if (node.Nodes.ContainsKey(key))
        //        {
        //            // Add removal code

        //        }
        //    }

        //    index = tree.Nodes.IndexOfKey("files");
        //    if (index > -1)
        //    {
        //        node = tree.Nodes[index];
        //        if (node.Nodes.ContainsKey(key))
        //        {
        //            // Add removal code

        //        }
        //    }

        //}

    }
}
