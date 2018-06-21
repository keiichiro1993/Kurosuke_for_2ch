using Kurosuke_for_2ch.Models;
using Microsoft.Data.Entity.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace Kurosuke_for_2ch.Utils
{
    public class StoreToFiles
    {
        public StoreToFiles()
        {
        }

        public async Task<List<Category>> LoadCategories()
        {
            using (var db = new ThreadContext())
            {
                await db.Database.EnsureCreatedAsync();
                try
                {
                    return db.Categories.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async Task AddCategories(IEnumerable<Category> categories)
        {
            using (var db = new ThreadContext())
            {
                foreach (var category in categories)
                {
                    var dbcate = db.Add(category);
                    foreach (var ita in category.Itas)
                    {
                        ita.CategoryId = dbcate.Entity.CategoryId;
                        db.Add(ita);
                    }
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<Ita>> AddOrUpdateCategories(IEnumerable<Category> categories)
        {
            var changedIta = new List<Ita>();
            using (var db = new ThreadContext())
            {
                var dbCategories = db.Categories;
                foreach (var category in categories)
                {
                    var dbCategory = (from dbcate in dbCategories
                                      where dbcate.Name == category.Name
                                      select dbcate).FirstOrDefault();
                    if (dbCategory == null)
                    {
                        var addedcate = db.Add(category);
                        foreach (var ita in category.Itas)
                        {
                            ita.CategoryId = addedcate.Entity.CategoryId;
                            db.Add(ita);
                        }
                    }
                    else
                    {
                        foreach (var ita in category.Itas)
                        {
                            var dbexistIta = (from dbIta in db.Itas
                                              where dbIta.Name == ita.Name
                                              select dbIta).FirstOrDefault();
                            if (dbexistIta == null)
                            {
                                ita.CategoryId = dbCategory.CategoryId;
                                db.Add(ita);
                            }
                            else
                            {
                                if (ita.Url != dbexistIta.Url)
                                {
                                    dbexistIta.UpdateInformation(ita);
                                    db.Update(dbexistIta);
                                    changedIta.Add(dbexistIta);
                                }
                            }
                        }
                    }
                }
                await db.SaveChangesAsync();
            }
            return changedIta;
        }

        public ObservableCollection<Ita> LoadItas(Category category)
        {
            using (var db = new ThreadContext())
            {
                var itas = from ita in db.Itas
                           where ita.CategoryId == category.CategoryId
                           select ita;

                var returnItas = new ObservableCollection<Ita>();
                foreach (var ita in itas)
                {
                    ita.Category = category;
                    ita.CategoryId = category.CategoryId;
                    returnItas.Add(ita);
                }
                return returnItas;
            }
        }

        public async Task<ObservableCollection<Post>> AddPostsWithMidokuCount(ObservableCollection<Post> newPosts, Thread thread)
        {
            int beforeCount;
            using (var db = new ThreadContext())
            {
                var beforePosts = from post in db.Posts
                                  where post.ThreadId == thread.ThreadId
                                  select post;
                beforeCount = beforePosts.Count();
            }
            var afterPosts = await AddPosts(newPosts, thread);
            var midoku = afterPosts.Count() - beforeCount;
            if (midoku > 0)
            {
                thread.MidokuCount = midoku;
                await AddOrUpdateThreadInformation(thread);
            }
            return afterPosts;
        }

        public async Task<ObservableCollection<Post>> AddPosts(ObservableCollection<Post> newPosts, Thread thread)
        {
            using (var db = new ThreadContext())
            {
                var posts = (from post in db.Posts
                             where post.ThreadId == thread.ThreadId
                             orderby post.DataId
                             select post).ToList();

                IEnumerable<Post> addPosts;
                if (posts.Count() > 0)
                {
                    addPosts = (from newPost in newPosts
                                where newPost.DataId > posts.Last().DataId
                                select newPost).ToList();
                }
                else
                {
                    addPosts = newPosts;
                }

                foreach (var newPost in addPosts)
                {
                    newPost.ThreadId = thread.ThreadId;
                    newPost.Thread = thread;
                }

                if (addPosts != null)
                {
                    foreach (var addpost in addPosts)
                    {
                        db.Add(addpost);
                    }
                }

                await db.SaveChangesAsync();

                posts = (from post in db.Posts
                         where post.ThreadId == thread.ThreadId
                         orderby post.DataId
                         select post).ToList();

                var returnPosts = new ObservableCollection<Post>();
                foreach (var post in posts)
                {
                    returnPosts.Add(post);
                }
                return returnPosts;
            }
        }

        public ObservableCollection<Post> LoadPosts(Thread thread)
        {
            using (var db = new ThreadContext())
            {
                /*var tmpthread = (from dbthread in db.Threads
                                 where dbthread.ThreadId == thread.ThreadId
                                 select dbthread).FirstOrDefault();*/
                //var posts = tmpthread.Posts;

                var posts = from post in db.Posts
                            where post.ThreadId == thread.ThreadId
                            orderby post.DataId
                            select post;
                thread.Posts = new ObservableCollection<Post>(posts);
                return thread.Posts;
            }
        }

        public async Task UpdateCurrentOffset(Thread updatedThread)
        {
            using (var db = new ThreadContext())
            {
                var threads = db.Threads;
                var exist = (from thread in threads
                             where (thread.Name == updatedThread.Name) && (thread.ItaId == updatedThread.ItaId) && (thread.Ita.CategoryId == updatedThread.Ita.CategoryId)
                             select thread).FirstOrDefault();

                exist.UpdateOffset(updatedThread);
                db.Update(exist);
                await db.SaveChangesAsync();
            }
        }

        public async Task<Thread> AddOrUpdateThreadInformation(Thread additionalThread)
        {
            using (var db = new ThreadContext())
            {
                var threads = db.Threads;
                var exist = (from thread in threads
                             where (thread.Name == additionalThread.Name) && (thread.ItaId == additionalThread.ItaId) && (thread.Ita.CategoryId == additionalThread.Ita.CategoryId)
                             select thread).FirstOrDefault();
                EntityEntry<Thread> returnThreadEntry;
                if (exist != null)
                {
                    exist.UpdateInformation(additionalThread);
                    returnThreadEntry = db.Update(exist);
                }
                else
                {
                    exist = additionalThread;
                    returnThreadEntry = db.Add(exist);
                }

                await db.SaveChangesAsync();

                var returnThread = returnThreadEntry.Entity;
                if (returnThread.Ita == null)
                {
                    returnThread.Ita = (from ita in db.Itas
                                        where ita.ItaId == returnThread.ItaId
                                        select ita).FirstOrDefault();
                }
                return returnThread;
            }
        }

        public ObservableCollection<Thread> LoadThreads(ObservableCollection<Thread> threadList)
        {
            threadList.Clear();
            using (var db = new ThreadContext())
            {
                var itas = db.Itas;

                foreach (var ita in itas)
                {
                    var threads = from thread in db.Threads
                                  where thread.ItaId == ita.ItaId
                                  select thread;
                    if (threads.Count() > 0)
                    {
                        foreach (var thread in threads)
                        {
                            threadList.Add(thread);
                        }
                    }
                }
                return threadList;
            }
        }

        public float LoadOffset(Thread thread)
        {
            using (var db = new ThreadContext())
            {
                var dbThread = (from dbth in db.Threads
                                where dbth.ThreadId == thread.ThreadId
                                select dbth).FirstOrDefault();
                return dbThread.CurrentOffset;
            }
        }

        public async void DeleteThread(Thread thread)
        {
            using (var db = new ThreadContext())
            {
                var posts = from post in db.Posts
                            where post.ThreadId == thread.ThreadId
                            select post;
                db.RemoveRange(posts);
                db.Remove(thread);

                await db.SaveChangesAsync();
            }
        }
    }
}
