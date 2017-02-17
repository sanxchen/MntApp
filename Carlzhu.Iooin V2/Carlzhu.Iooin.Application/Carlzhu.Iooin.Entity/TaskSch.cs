﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carlzhu.Iooin.Entity
{
   public class TaskSch
    {
        public TaskSch()
        {
            this.Tasks1 = new HashSet<TaskSch>();
        }

        [Key]
        public int TaskID { get; set; }
        public System.DateTime Start { get; set; }
        public System.DateTime End { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> OwnerID { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; }
        public Nullable<int> RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }

        public virtual ICollection<TaskSch> Tasks1 { get; set; }
        public virtual TaskSch Task1 { get; set; }

    }
}
