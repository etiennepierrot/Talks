using System;
using System.Threading;
using Presentation.CommandHandlers;
using Xunit;

namespace Presentation.Tests
{
    public class LifeTimeTests
    {
        [Fact]
        public void PerThreadShouldResolveTheSameForOneThread()
        {
            Func<FakeRepository> repositoryFactory = () => new FakeRepository();

            using (var lifeTime = new LifeTime())
            {
                var fakeRepo1 = lifeTime.PerThread(repositoryFactory);
                var fakeRepo2 = lifeTime.PerThread(repositoryFactory);
                Assert.Same(fakeRepo1, fakeRepo2);
            }
        }

        [Fact]
        public void PerThreadShouldResolveOncePerThread()
        {
            Func<FakeRepository> repositoryFactory = () => new FakeRepository();

            using (var lifeTime = new LifeTime())
            {
                FakeRepository repo1 = null;
                FakeRepository repo2 = null;

                var t1 = new Thread(() =>
                        {
                            repo1 = lifeTime.PerThread(repositoryFactory);
                        });

                var t2 = new Thread(() =>
                        {
                            repo2 = lifeTime.PerThread(repositoryFactory);
                        });

                t1.Start();
                t2.Start();

                t1.Join();
                t2.Join();

                Assert.NotNull(repo1);
                Assert.NotNull(repo2);
                Assert.NotSame(repo1, repo2);
            }
        }

        [Fact]
        public void PerThreadShouldDisposeOncePerThread()
        {
            Func<FakeRepository> repositoryFactory = () => new FakeRepository();

            var lifeTime = new LifeTime();
            FakeRepository repo1 = null;
            FakeRepository repo2 = null;

            var t1 = new Thread(() =>
            {
                repo1 = lifeTime.PerThread(repositoryFactory);
            });

            var t2 = new Thread(() =>
            {
                repo2 = lifeTime.PerThread(repositoryFactory);
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            
            lifeTime.Dispose();

            Assert.True(repo1.IsDispose);
            Assert.True(repo2.IsDispose);

        }
    }

    public class FakeRepository : IDisposable
    {
        public bool IsDispose { get; private set; }

        public FakeRepository()
        {
            IsDispose = false;
            InitializedTimes++;
        }

        public int InitializedTimes { get; private set; }

        public void Dispose()
        {
            IsDispose = true;
        }
    }
}