﻿using Ecomm_Project4_4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm_Project4_4.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository:IRepository<Category>
    {
        void Update(Category category);
    }
}
