using System;
using System.Collections.Generic;
using System.Text;
using Tarantula.MVP.Events;

namespace Tarantula.MVP.View
{
    public interface IBookDetailView: IView
    {
        event BookEventHandler Hidden;

        string Author { set; }
        string ItemID { set; }
        string ImageURL { set;}
        string Title { set; }
        string DetailURL { set; }
        string LowestNewPrice { set; }
        string LowestUsedPrice { set; }

        void Show();
        void Hide(string itemID);
    }
}
