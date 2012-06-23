using System;
using System.Collections.Generic;
using System.Text;

namespace Physics
{
    public delegate void MetaDataDisposeHandler<M>(object sender,MetaDataDisposeEventArgs<M> metaData);

    public class MetaDataDisposeEventArgs<M>: EventArgs
    {
        private M _metaData;

        public MetaDataDisposeEventArgs(M metaData)
        {
            _metaData = metaData;
        }

        public M MetaData
        {
            get { return _metaData; }
            set { _metaData = value; }
        }
    }
}
