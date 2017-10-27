﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface INavigationService
    {
        void PushView(string key);
        void GoBack();
    }
}
