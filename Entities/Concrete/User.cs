using System;
using System.Collections.Generic;
using System.Security.Claims;
using Core.Entities;

namespace Entities.Concrete
{
    public class User : BaseEntity, IEntity
    {
        public string Name { get; set; }
    }
}
