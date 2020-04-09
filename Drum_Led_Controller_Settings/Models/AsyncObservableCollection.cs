using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drum_Led_Controller_Settings.Models
{
    /*   public class AsyncObservableCollection<T> : ObservableCollection<T>
       {
           private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

           public AsyncObservableCollection()
           {
           }

           public AsyncObservableCollection(IEnumerable<T> list)
               : base(list)
           {
           }

           protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
           {
               if (SynchronizationContext.Current == _synchronizationContext)
               {
                   // Execute the CollectionChanged event on the current thread
                   RaiseCollectionChanged(e);
               }
               else
               {
                   // Raises the CollectionChanged event on the creator thread
                   _synchronizationContext.Send(RaiseCollectionChanged, e);
               }
           }

           private void RaiseCollectionChanged(object param)
           {
               // We are in the creator thread, call the base implementation directly
               base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
           }

           protected override void OnPropertyChanged(PropertyChangedEventArgs e)
           {
               if (SynchronizationContext.Current == _synchronizationContext)
               {
                   // Execute the PropertyChanged event on the current thread
                   RaisePropertyChanged(e);
               }
               else
               {
                   // Raises the PropertyChanged event on the creator thread
                   _synchronizationContext.Send(RaisePropertyChanged, e);
               }
           }

           private void RaisePropertyChanged(object param)
           {
               // We are in the creator thread, call the base implementation directly
               base.OnPropertyChanged((PropertyChangedEventArgs)param);
           }
       }*/

    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        public AsyncObservableCollection()
        {
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
        }

        private void ExecuteOnSyncContext(Action action)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                action();
            }
            else
            {
                _synchronizationContext.Send(_ => action(), null);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            ExecuteOnSyncContext(() => base.InsertItem(index, item));
        }

        protected override void RemoveItem(int index)
        {
            ExecuteOnSyncContext(() => base.RemoveItem(index));
        }

        protected override void SetItem(int index, T item)
        {
            ExecuteOnSyncContext(() => base.SetItem(index, item));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            ExecuteOnSyncContext(() => base.MoveItem(oldIndex, newIndex));
        }

        protected override void ClearItems()
        {
            ExecuteOnSyncContext(() => base.ClearItems());
        }
    }

}
