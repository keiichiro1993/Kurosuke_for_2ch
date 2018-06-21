using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Kurosuke_for_2ch.Models;

namespace Kurosuke_for_2ch.Migrations
{
    [DbContext(typeof(ThreadContext))]
    [Migration("20160329060704_migrate")]
    partial class migrate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("Kurosuke_for_2ch.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("CategoryId");
                });

            modelBuilder.Entity("Kurosuke_for_2ch.Models.Ita", b =>
                {
                    b.Property<int>("ItaId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("ItaId");
                });

            modelBuilder.Entity("Kurosuke_for_2ch.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DataDate");

                    b.Property<int>("DataId");

                    b.Property<string>("DataUserId");

                    b.Property<string>("Date");

                    b.Property<int>("Id");

                    b.Property<string>("MailTo");

                    b.Property<string>("Message");

                    b.Property<string>("Name");

                    b.Property<string>("Number");

                    b.Property<int>("ThreadId");

                    b.HasKey("PostId");
                });

            modelBuilder.Entity("Kurosuke_for_2ch.Models.Thread", b =>
                {
                    b.Property<int>("ThreadId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Count");

                    b.Property<float>("CurrentOffset");

                    b.Property<int>("ItaId");

                    b.Property<int>("ItaNumber");

                    b.Property<int>("MidokuCount");

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("ThreadId");
                });

            modelBuilder.Entity("Kurosuke_for_2ch.Models.Ita", b =>
                {
                    b.HasOne("Kurosuke_for_2ch.Models.Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("Kurosuke_for_2ch.Models.Post", b =>
                {
                    b.HasOne("Kurosuke_for_2ch.Models.Thread")
                        .WithMany()
                        .HasForeignKey("ThreadId");
                });

            modelBuilder.Entity("Kurosuke_for_2ch.Models.Thread", b =>
                {
                    b.HasOne("Kurosuke_for_2ch.Models.Ita")
                        .WithMany()
                        .HasForeignKey("ItaId");
                });
        }
    }
}
