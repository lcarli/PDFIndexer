﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PDFIndexer.Execution
{
    class PipelineDisposeHelper : IDisposable
    {
        private List<IDisposable> _disposableObjects = new List<IDisposable>();

        public void TrackInstance(object instance)
        {
            var disposableObj = instance as IDisposable;
            if (disposableObj != null)
            {
                _disposableObjects.Add(disposableObj);
            }
        }

        public void FreeObject(object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public void Dispose()
        {
            lock (_disposableObjects)
            {
                if (_disposableObjects != null)
                {
                    foreach (var obj in _disposableObjects)
                    {
                        FreeObject(obj);
                    }

                    _disposableObjects = null;
                }
            }
        }
    }
}
