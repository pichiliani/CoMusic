//
// GT: The Groupware Toolkit for C#
// Copyright (C) 2006 - 2009 by the University of Saskatchewan
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later
// version.
// 
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA
// 02110-1301  USA
// 

using System;

namespace GT
{
    /// <summary>
    /// Defines methods for starting, stopping, and disposing of an instance.
    /// A stopped instance may be started again.  A disposed instance can never
    /// be restarted.  A stopped instance can be stopped multiple times.
    /// To properly support IDisposable, an instance can be disposed whether it
    /// is started or stopped.
    /// </summary>
    public interface IStartable : IDisposable
    {
        /// <summary>
        /// Start the instance.  Starting an instance may throw an exception on error.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the instance.  Instances can be stopped multiple times.
        /// Stopping an instance may throw an exception on error.
        /// </summary>
        void Stop();

        /// <summary>
        /// Return true if the instance is currently active.
        /// </summary>
        bool Active { get; }
    }
}
