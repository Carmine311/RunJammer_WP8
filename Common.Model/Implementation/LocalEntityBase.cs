using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Model.Interface;

namespace Common.Model.Implementation
{
	public abstract class LocalEntityBase : ILocalEntity
	{
		public abstract int ID { get; set; }
		public abstract int LocalID { get; set; }
	}
}
