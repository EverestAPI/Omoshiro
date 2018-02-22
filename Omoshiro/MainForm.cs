using System;
using Eto.Forms;
using Eto.Drawing;

namespace Omoshiro {
    public class MainForm : Form {

        public readonly static Version Version = new Version(0, 0, 0);

        public GhostData Data;

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

        public MainForm() {
            Title = "Omoshiro";
            ClientSize = new Size(600, 500);

            Icon = Icon.FromResource("Omoshiro.Content.icon.ico");

            Content = new StackLayout {
                Padding = 8,
                Items = {

                    new GroupBox {
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
                    },

                    
                }
            };

            textBoxSID.TextChanged += (sender, args) => Data.SID = textBoxSID.Text;
            dropDownMode.SelectedIndexChanged += (sender, args) => Data.Mode = (AreaMode) dropDownMode.SelectedIndex;
            textBoxLevel.TextChanged += (sender, args) => Data.Level = textBoxLevel.Text;
            textBoxTarget.TextChanged += (sender, args) => Data.Target = textBoxTarget.Text;
            textBoxName.TextChanged += (sender, args) => Data.Name = textBoxName.Text;
            dateTimePicker.ValueChanged += (sender, args) => Data.Date = dateTimePicker.Value ?? Data.Date;
            checkBoxDead.CheckedChanged += (sender, args) => Data.Dead = checkBoxDead.Checked ?? Data.Dead;
            sliderOpacity.ValueChanged += (sender, args) => {
                Data.Opacity = sliderOpacity.Value == 11 ? (float?) null : (sliderOpacity.Value / 10f);
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
                Items =
                {
                    new ButtonMenuItem { Text = "&File", Items = {
                            cmdNew,
                            cmdOpen,
                            cmdSave,
                            cmdSaveAs
                    } },
                },
                QuitItem = cmdQuit,
                AboutItem = cmdAbout
            };

            Command cmdFrameAdd = new Command { Image = Icon.FromResource("Omoshiro.Content.add.ico"), ToolTip = "Add Frame", Shortcut = Application.Instance.CommonModifier | Keys.Plus };
            Command cmdFrameRemove = new Command { Image = Icon.FromResource("Omoshiro.Content.remove.ico"), ToolTip = "Remove Selection", Shortcut = Keys.Delete };

            ToolBar = new ToolBar { Items = {
                    cmdFrameAdd,
                    cmdFrameRemove
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
            Data = new GhostData();
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
            Data = data;
            RefreshView();
        }

        public void DataSave() {
            if (Data.FilePath == null) {
                DataSaveAs();
                return;
            }
            Data.Write();
        }

        public void DataSaveAs() {
            SaveFileDialog dialog = new SaveFileDialog {
                FileName = Data.FilePath,
                Filters = {
                    new FileDialogFilter("Everest Ghost Mod File (*.oshiro)", ".oshiro")
                }
            };
            DialogResult result = dialog.ShowDialog(this);
            if (result != DialogResult.Ok) {
                return;
            }
            Data.FilePath = dialog.FileName;
            DataSave();
        }

        public void RefreshView() {
            textBoxSID.Text = Data.SID;
            dropDownMode.SelectedIndex = (int) Data.Mode;
            textBoxLevel.Text = Data.Level;
            textBoxTarget.Text = Data.Target;
            textBoxName.Text = Data.Name;
            dateTimePicker.Value = Data.Date;
            checkBoxDead.Checked = Data.Dead;
            RefreshViewOpacity();

            // TODO: Refresh view.
        }

        public void RefreshViewOpacity() {
            labelOpacity.Text = $"Opacity - {( Data.Opacity == null ? "Default" : (((int) Math.Round(Data.Opacity.Value * 100)) + "%") )}";
            sliderOpacity.Value = Data.Opacity == null ? 11 : (int) Math.Round(Data.Opacity.Value * 10);
        }

    }
}