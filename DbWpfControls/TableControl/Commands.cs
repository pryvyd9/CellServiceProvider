using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbWpfControls.TableControl
{

    public class TaskCancellationToken
    {
        public bool IsCancellationRequested { get; set; }
    }

    public abstract class TableCommand
    {
        protected readonly TableControl control;

        protected TableCommand(TableControl control)
        {
            this.control = control;
        }

        public abstract Task ExecuteAsync(TaskCancellationToken cancellationToken);
    }

    //public class BlockButtons : TableCommand
    //{
    //    public BlockButtons(TableControl control) : base(control)
    //    {
    //    }

    //    public override Task ExecuteAsync(TaskCancellationToken cancellationToken)
    //    {
    //        foreach (var button in control.ControlButtons)
    //        {
    //            button.IsEnabled = false;
    //        }

    //        while (true)
    //        {

    //        }

    //        throw new NotImplementedException();
    //    }
    //}

    public class RefreshCommand : TableCommand
    {
        public RefreshCommand(TableControl control) : base(control) {}

        public override async Task ExecuteAsync(TaskCancellationToken cancellationToken = null)
        {
            control.Refresh();
        }

    }

    public class SelectCommand : TableCommand
    {
        private readonly int rowCount;

        private readonly List<Cell> selectedCells = new List<Cell>();

        public IEnumerable<IReadOnlyDictionary<string, object>> SelectedRows =>
            selectedCells.Distinct().Select(n => control.table.GetOldValues(n));

        public SelectCommand(int rowCount, TableControl control) : base(control)
        {
            this.rowCount = rowCount;
        }

        public override async Task ExecuteAsync(TaskCancellationToken cancellationToken = null)
        {
            control.table.CellSelected += Table_CellSelected;

            while (selectedCells.Count < rowCount)
            {
                if (cancellationToken?.IsCancellationRequested ?? false)
                {
                    throw new TaskCanceledException();
                }

                await Task.Delay(100);
            }

            control.table.CellSelected -= Table_CellSelected;
        }

        private void Table_CellSelected(object sender)
        {
            selectedCells.Add((Cell)sender);
        }
    }

    public class DeleteCommand : TableCommand
    {
        private readonly int rowCount;

        public DeleteCommand(int rowCount, TableControl control) : base(control)
        {
            this.rowCount = rowCount;
        }

        public override async Task ExecuteAsync(TaskCancellationToken cancellationToken = null)
        {
            var selection = new SelectCommand(rowCount, control);

            var task = selection.ExecuteAsync(cancellationToken);

            await task;

            if (task.IsCanceled)
            {
                throw new TaskCanceledException();
            }

            if (!control.TryDelete(selection.SelectedRows, out var exception))
            {
                Console.WriteLine($"{exception.Message}\n{exception.StackTrace}");
            }
        }
    }

    public class CommitCommand : TableCommand
    {
        public CommitCommand(TableControl control) : base(control)
        {
        }

        public override async Task ExecuteAsync(TaskCancellationToken cancellationToken = null)
        {
            if (!control.TryCommit(out var exception))
            {
                Console.WriteLine($"{exception.Message}\n{exception.StackTrace}");
            }
        }
    }

    public class InsertCommand : TableCommand
    {
        public IReadOnlyDictionary<string, object> Values { get; }

        public InsertCommand(TableControl control, IReadOnlyDictionary<string, object> values)
            : base(control)
        {
            Values = values;
        }

        public override async Task ExecuteAsync(TaskCancellationToken cancellationToken = null)
        {
            if (!control.TryInsert(Values, out var exception))
            {
                Console.WriteLine($"{exception.Message}\n{exception.StackTrace}");
            }
            else
            {
                control.insertBox.Reset();
            }
        }
    }
}
