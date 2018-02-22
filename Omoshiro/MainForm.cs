﻿using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Omoshiro {
    public class MainForm : Form {

        public readonly static Version Version = new Version(0, 0, 0);

        public GhostData Ghost;

        Command cmdNew;
        Command cmdOpen;
        Command cmdSave;
        Command cmdSaveAs;

        TextBox textBoxSID;
        DropDown dropDownMode;
        TextBox textBoxLevel;
        TextBox textBoxTarget;
        TextBox textBoxName;
        DateTimePicker dateTimePicker;
        CheckBox checkBoxDead;
        Label labelOpacity;
        Slider sliderOpacity;

        GridView gridViewFrames;

        public MainForm() {
            Title = "Omoshiro";
            ClientSize = new Size(1090, 500);

            Icon = Icon.FromResource("Omoshiro.Content.icon.ico");

            DynamicLayout root = new DynamicLayout {
                Padding = 8
            };
            Content = root;

            root.BeginHorizontal();
            root.Add(new GroupBox {
                Text = "Metadata",
                Content = new TableLayout {
                    Padding = 8,
                    Spacing = new Size(2, 2),
                    Rows = {
                                new TableRow { Cells = {
                                        "SID",
                                        (textBoxSID = new TextBox()),
                                } },

                                new TableRow { Cells = {
                                        "Mode",
                                        (dropDownMode = new DropDown { Items = {
                                                AreaMode.Normal.ToString(),
                                                AreaMode.BSide.ToString(),
                                                AreaMode.CSide.ToString()
                                        } }),
                                } },

                                new TableRow { Cells = {
                                        "Level",
                                        (textBoxLevel = new TextBox()),
                                } },

                                new TableRow { Cells = {
                                        "Target",
                                        (textBoxTarget = new TextBox()),
                                } },

                                new TableRow { Cells = {
                                        "Author",
                                        (textBoxName = new TextBox()),
                                } },

                                new TableRow { Cells = {
                                        "Created at",
                                        (dateTimePicker = new DateTimePicker { Mode = DateTimePickerMode.DateTime }),
                                } },

                                new TableRow { Cells = {
                                        "Ends with death",
                                        (checkBoxDead = new CheckBox()),
                                } },

                                new TableRow { Cells = {
                                        (labelOpacity = new Label { Text = "Opacity", Width = 128 }),
                                        (sliderOpacity = new Slider { MinValue = 0, MaxValue = 11, SnapToTick = true }),
                                } },
                            },
                },
            });
            root.Add(new Scrollable {
                Content = (gridViewFrames = new GridView {
                    ShowHeader = true,
                    GridLines = GridLines.Both,
                    AllowColumnReordering = false,
                    AllowMultipleSelection = true,
                    Columns = {
                        new GridColumn {
                            HeaderText = "#",
                            Editable = false
                        },
                        new GridColumn {
                            HeaderText = "Data?",
                            Editable = false,
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "↔",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "↕",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Aim",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Mountain Aim",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "ESC",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Pause",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Quick Restart",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Jump",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Dash",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Grab",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Talk",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "Menu",
                            Editable = false,
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "←",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "→",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "↑",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "↓",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "✓",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "❌",
                            Sortable = false,
                        },
                        new GridColumn {
                            HeaderText = "📖",
                            Sortable = false,
                        },
                    }
                })
            });

            textBoxSID.TextChanged += (sender, e) => Ghost.SID = textBoxSID.Text;
            dropDownMode.SelectedIndexChanged += (sender, e) => Ghost.Mode = (AreaMode) dropDownMode.SelectedIndex;
            textBoxLevel.TextChanged += (sender, e) => Ghost.Level = textBoxLevel.Text;
            textBoxTarget.TextChanged += (sender, e) => Ghost.Target = textBoxTarget.Text;
            textBoxName.TextChanged += (sender, e) => Ghost.Name = textBoxName.Text;
            dateTimePicker.ValueChanged += (sender, e) => Ghost.Date = dateTimePicker.Value ?? Ghost.Date;
            checkBoxDead.CheckedChanged += (sender, e) => Ghost.Dead = checkBoxDead.Checked ?? Ghost.Dead;
            sliderOpacity.ValueChanged += (sender, e) => {
                Ghost.Opacity = sliderOpacity.Value == 11 ? (float?) null : (sliderOpacity.Value / 10f);
                RefreshViewOpacity();
            };

            cmdNew = new Command { MenuText = "&New", Shortcut = Application.Instance.CommonModifier | Keys.N };
            cmdNew.Executed += (sender, e) => DataNew();
            cmdOpen = new Command { MenuText = "&Open", Shortcut = Application.Instance.CommonModifier | Keys.O};
            cmdOpen.Executed += (sender, e) => DataOpen();
            cmdSave = new Command { MenuText = "&Save", Shortcut = Application.Instance.CommonModifier | Keys.S};
            cmdSave.Executed += (sender, e) => DataSave();
            cmdSaveAs = new Command { MenuText = "Save As", Shortcut = Application.Instance.CommonModifier | Keys.Shift | Keys.S };
            cmdSaveAs.Executed += (sender, e) => DataSaveAs();

            Command cmdQuit = new Command { MenuText = "&Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            cmdQuit.Executed += (sender, e) => Application.Instance.Quit();

            Command cmdAbout = new Command { MenuText = $"Omoshiro v.{Version}", Enabled = false };

            Menu = new MenuBar {
                Items = {
                    new ButtonMenuItem {
                        Text = "&File",
                        Items = {
                            cmdNew,
                            cmdOpen,
                            cmdSave,
                            cmdSaveAs
                        }
                    },
                },
                QuitItem = cmdQuit,
                AboutItem = cmdAbout
            };

            Command cmdFrameAdd = new Command { ToolBarText = " + ", ToolTip = "Add frame (Insert)", Shortcut = Keys.Insert };
            cmdFrameAdd.Executed += (sender, e) => {
                // TODO: Take selected index into account.
                int index = Ghost.Frames.Count;
                foreach (int selected in gridViewFrames.SelectedRows) {
                    index = selected;
                    break;
                }
                Ghost.Frames.Insert(index, new GhostFrame { HasInput = true });
                RefreshViewFrames();
            };
            Command cmdFrameRemove = new Command { ToolBarText = " - ", ToolTip = "Remove selection (Delete)", Shortcut = Keys.Delete };
            Command cmdFrameMoveUp = new Command { ToolBarText = " ↑ ", ToolTip = "Move selection up (Shift + Up)", Shortcut = Keys.Shift | Keys.Up };
            Command cmdFrameMoveDown = new Command { ToolBarText = " ↓ ", ToolTip = "Move selection down (Shift + Down)", Shortcut = Keys.Shift | Keys.Down };

            ToolBar = new ToolBar { Items = {
                    cmdFrameAdd,
                    cmdFrameRemove,
                    cmdFrameMoveUp,
                    cmdFrameMoveDown
            } };

            DataNew();
        }

        public StackLayout LabelControlPair(string label, Control control, Orientation orientation = Orientation.Vertical)
            => LabelControlPair(new Label { Text = label }, control, orientation);
        public StackLayout LabelControlPair(Label label, Control control, Orientation orientation = Orientation.Vertical)
            => new StackLayout {
                Padding = 2,
                Orientation = orientation,
                Items = {
                    label,
                    control
                }
            };

        public void DataNew() {
            Ghost = new GhostData();
            Ghost.Frames.Add(new GhostFrame { HasInput = true });
            RefreshView();
        }

        public void DataOpen() {
            OpenFileDialog dialog = new OpenFileDialog {
                Filters = {
                    new FileDialogFilter("Everest Ghost Mod File (*.oshiro)", ".oshiro")
                }
            };
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.Ok) {
                return;
            }
            GhostData data = new GhostData(dialog.FileName).Read();
            if (data == null) {
                MessageBox.Show(this, "The selected .oshiro file is not compatible with Omoshiro.", "Unsupported / invalid .oshiro", MessageBoxType.Error);
                return;
            }
            Ghost = data;
            RefreshView();
        }

        public void DataSave() {
            if (Ghost.FilePath == null) {
                DataSaveAs();
                return;
            }
            Ghost.Write();
        }

        public void DataSaveAs() {
            SaveFileDialog dialog = new SaveFileDialog {
                FileName = Ghost.FilePath,
                Filters = {
                    new FileDialogFilter("Everest Ghost Mod File (*.oshiro)", ".oshiro")
                }
            };
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.Ok) {
                return;
            }
            Ghost.FilePath = dialog.FileName;
            DataSave();
        }

        public void RefreshView() {
            textBoxSID.Text = Ghost.SID;
            dropDownMode.SelectedIndex = (int) Ghost.Mode;
            textBoxLevel.Text = Ghost.Level;
            textBoxTarget.Text = Ghost.Target;
            textBoxName.Text = Ghost.Name;
            dateTimePicker.Value = Ghost.Date;
            checkBoxDead.Checked = Ghost.Dead;

            RefreshViewOpacity();
            RefreshViewFrames();
        }

        public void RefreshViewOpacity() {
            labelOpacity.Text = $"Opacity - {( Ghost.Opacity == null ? "Default" : (((int) Math.Round(Ghost.Opacity.Value * 100)) + "%") )}";
            sliderOpacity.Value = Ghost.Opacity == null ? 11 : (int) Math.Round(Ghost.Opacity.Value * 10);
        }

        public void RefreshViewFrames() {
            List<object> data = new List<object>();
            for (int i = 0; i < Ghost.Frames.Count; i++) {
                GhostFrame frame = Ghost[i];
                data.Add(frame);
            }
            gridViewFrames.DataStore = data;
        }

    }
}