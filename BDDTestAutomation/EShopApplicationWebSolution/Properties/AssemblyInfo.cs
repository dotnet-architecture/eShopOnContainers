// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Microsoft">
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Resources;

//// using Microsoft.VisualStudio.TestTools.UnitTesting;

using NUnit.Framework;

//// using XUnit.Extensibility.Core;

[assembly: Parallelizable(ParallelScope.Fixtures)] // NUnit
//// [assembly: Parallelize(Scope = ExecutionScope.ClassLevel)] // MSTest
//// [assembly: CollectionBehavior(CollectionBehavior.CollectionPerClass)] // XUnit

[assembly: CLSCompliant(false)]
[assembly: NeutralResourcesLanguage("en-US")]