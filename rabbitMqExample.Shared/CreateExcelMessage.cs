﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rabbitMqExample.Shared {
    public class CreateExcelMessage {
        public int FileId { get; set; }
        public string UserId { get; set; }
    }
}
