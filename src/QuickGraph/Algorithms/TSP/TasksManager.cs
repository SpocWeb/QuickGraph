using QuickGraph.Collections;
using System;

namespace QuickGraph.Algorithms.TSP
{
    class TasksManager<TVertex, TEdge>
        where TEdge : EquatableEdge<TVertex>
    {
        private readonly BinaryHeap<TaskPriority, Task<TVertex, TEdge>> _tasksQueue;


        public TasksManager()
        {
            _tasksQueue = new BinaryHeap<TaskPriority, Task<TVertex, TEdge>>();
        }

        public void AddTask(Task<TVertex, TEdge> task)
        {
            if (task.MinCost < double.PositiveInfinity)
            {
                _tasksQueue.Add(task.Priority, task);
            }
        }

        public Task<TVertex, TEdge> GetTask()
        {
            return _tasksQueue.RemoveMinimum().Value;
        }

        public bool HasTasks()
        {
            return _tasksQueue.Count > 0 ;
        }
    }

}
