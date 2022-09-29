using System;
using Business.Abstract;
using Constants.Message;
using Business.ValidationRules.FluentValidation;
using Core.Utilities.Hash;
using Core.Utilities.Result;
using DataAccess.Abstract;
using Entities.Concrete;
using FluentValidation;
using Core.CrossCuttingConcerns.Validation;
using Core.Aspects.Autofac.Validation;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PagedList.Core;
using System.Linq.Expressions;
using Core.FrontEnd;
using System.Data.Common;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }


        [ValidationAspect(typeof(UserValidator))]
        public IDataResult<User> AddUser(User user)
        {
            _userDal.Add(user);

            throw new NotImplementedException();
        }

        public IDataResult<User> GetUser(int Id)
        {

            var result = _userDal.Get(x => x.Id == Id && !x.IsDeleted);

            if (result == null)
            {
                return new ErrorDataResult<User>(null, Messages.RecordNotFound);
            }

            return new SuccessDataResult<User>(result);
        }

        public IResult User_Delete(User user)
        {
            _userDal.Delete(user);
            return new SuccessResult();
        }



    }
}

