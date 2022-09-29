using System;
using System.Linq.Expressions;
using Core.DataAccess;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Security.Cryptography;
using Core.Utilities.Hash;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete
{
    public class UserDal : EfEntityRepositoryBase<User, BuilderContext>, IUserDal
    {
    }
}
