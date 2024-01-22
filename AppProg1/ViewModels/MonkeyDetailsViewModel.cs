using AppProg1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProg1.ViewModels
{
    [QueryProperty(nameof(Monkey),nameof(Monkey))]

    public class MonkeyDetailsViewModel : BaseViewModel
    {
		private Monkey _monkey;

		public Monkey Monkey
		{
			get { return _monkey; }
			set { SetField(ref _monkey, value); }
		}

	}
}
