using System;
using System.Collections.Generic;
using Core.FrontEnd;
using Core.Utilities.Result;
using Entities.Concrete;
using PagedList.Core;

namespace Business.Abstract
{
    public interface IUserService
    {

        IDataResult<User> AddUser(User user);

        IDataResult<User> GetUser(int Id);

        IResult User_Delete(User user);


    }
}

