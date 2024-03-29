﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;

using Sys.Data;
using Sys.Stdio.Cli;
using Sys;

namespace sqlcli.Windows
{
    public class DbTreeUI : TreeView
    {
        private IPathManager mgr;
        public event EventHandler<EventArgs<TreeNode<IDataPath>>> PathChanged;

        public DbTreeUI()
        {
            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            this.ContextMenu = new ContextMenu();

            MenuItem menuItem = new MenuItem
            {
                Header = "Select Top 1000 Rows",
                Command = SqlCommands.Select1000,
                CommandParameter = 1000,
            };
            ContextMenu.Items.Add(menuItem);

            menuItem = new MenuItem
            {
                Header = "Select All Rows",
                Command = SqlCommands.Select,
                CommandParameter = 0,
            };
            ContextMenu.Items.Add(menuItem);

            ContextMenu.Items.Add(new Separator());

            menuItem = new MenuItem
            {
                Header = "Expand All",
            };
            menuItem.Click += (sender, e) => ApplyAllNodes((DbTreeNodeUI)this.SelectedItem, x => x.IsExpanded = true);
            ContextMenu.Items.Add(menuItem);

            menuItem = new MenuItem
            {
                Header = "Collapse All",
            };
            menuItem.Click += (sender, e) => ApplyAllNodes((DbTreeNodeUI)this.SelectedItem, x => x.IsExpanded = false);
            ContextMenu.Items.Add(menuItem);
        }


        internal void CreateTree(IConnectionConfiguration cfg, IPathManager mgr)
        {
            this.mgr = mgr;
            createTree(cfg, this);
        }

        public void chdir(IDataPath node)
        {
            switch (node)
            {
                case ServerName sname:
                    {
                        chdir($@"\{sname.Path}");
                    }
                    break;

                case DatabaseName dname:
                    {
                        ServerName sname = dname.ServerName;
                        chdir($@"\{sname.Path}\{dname.Path}");
                    }
                    break;

                case TableName tname:
                    {
                        DatabaseName dname = tname.DatabaseName;
                        ServerName sname = dname.ServerName;
                        chdir($@"\{sname.Path}\{dname.Path}\{tname.ShortName}");
                    }
                    break;
            }
        }

        public TreeNode<IDataPath> chdir(string path)
        {
            PathName pathName = new PathName(path);
            TreeNode<IDataPath> node = mgr.Navigate(pathName);
            if (node != null)
            {
                mgr.CurrentNode = node;
                PathChanged?.Invoke(this, new EventArgs<TreeNode<IDataPath>>(node));
            }

            return node;
        }

        public void ChangeNode(string path)
        {
            PathName pathName = new PathName(path);
            string[] S = pathName.FullSegments;

            if (S.Length < 2)
                return;

            foreach (DbServerNodeUI snode in this.Items)
            {
                ServerName sname = (ServerName)snode.Path;
                if (string.Compare(sname.Path, S[1], ignoreCase: true) != 0)
                    continue;

                snode.IsExpanded = true;
                snode.IsSelected = true;
                snode.ExpandNode();

                if (S.Length < 3)
                {
                    chdir(path);
                    return;
                }

                if (S[2] == "~")
                    S[2] = sname.DefaultDatabase.Name;

                foreach (DbDatabaseNodeUI dnode in snode.Items)
                {
                    DatabaseName dname = (DatabaseName)dnode.Path;
                    if (string.Compare(dname.Path, S[2], ignoreCase: true) != 0)
                        continue;

                    dnode.IsExpanded = true;
                    dnode.IsSelected = true;
                    dnode.ExpandNode();

                    if (S.Length < 4)
                    {
                        chdir(path);
                        return;
                    }

                    foreach (DbTableNodeUI tnode in dnode.Items)
                    {
                        TableName tname = (TableName)tnode.Path;
                        if (string.Compare(tname.Path, S[3], ignoreCase: true) != 0)
                            continue;

                        tnode.IsExpanded = true;
                        tnode.IsSelected = true;

                        chdir(path);
                        return;
                    }
                }
            }
        }

        private void createTree(IConnectionConfiguration cfg, TreeView treeView)
        {
            var L = cfg.Providers.OrderBy(x => x.ServerName.Path);
            foreach (var pvd in L)
            {
                ServerName sname = pvd.ServerName;
                DbTreeNodeUI item = new DbServerNodeUI(this, sname);

                treeView.Items.Add(item);
            }

            return;
        }

        public void RunFilter(string wildcard)
        {
            wildcard = wildcard.Trim();
            if (wildcard == string.Empty)
            {
                ShowAllNodes();
                return;
            }

            if (!wildcard.Contains('*') && !wildcard.Contains('?'))
                wildcard = $"*{wildcard}*";

            foreach (DbTreeNodeUI item in this.Items)
                SetVisibility(item, wildcard);
        }

        private bool SetVisibility(DbTreeNodeUI item, string wildcard)
        {
            bool matched = item.IsMatch(wildcard);
            //leave node
            if (item.Items.Count == 0)
                return SetVisibility(item, matched);

            //current node is matched
            if (matched)
            {
                foreach (DbTreeNodeUI theItem in item.Items)
                {
                    SetVisibility(theItem, wildcard);
                }

                //must be visible
                return SetVisibility(item, true);
            }
            else
            {
                bool found = false;
                foreach (DbTreeNodeUI theItem in item.Items)
                {
                    if (SetVisibility(theItem, wildcard))
                    {
                        if (!found)
                            found = true;
                    }
                }

                //be visible if any child node is visible
                return SetVisibility(item, found);
            }
        }

        private static bool SetVisibility(UIElement uiElement, bool visible)
        {
            if (visible)
                uiElement.Visibility = Visibility.Visible;
            else
                uiElement.Visibility = Visibility.Collapsed;

            return visible;
        }

        private void ShowAllNodes()
        {
            foreach (DbTreeNodeUI item in this.Items)
                ApplyAllNodes(item, x => x.Visibility = Visibility.Visible);
        }

        private void ApplyAllNodes(DbTreeNodeUI item, Action<DbTreeNodeUI> action)
        {
            if (item == null)
                return;

            action(item);
            foreach (DbTreeNodeUI theItem in item.Items)
            {
                ApplyAllNodes(theItem, action);
            }
        }
    }
}
