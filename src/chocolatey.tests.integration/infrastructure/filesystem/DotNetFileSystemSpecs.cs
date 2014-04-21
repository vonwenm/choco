﻿namespace chocolatey.tests.integration.infrastructure.filesystem
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using Should;
    using chocolatey.infrastructure.filesystem;

    public class DotNetFileSystemSpecs
    {
        public abstract class DotNetFileSystemSpecsBase : TinySpec
        {
            protected DotNetFileSystem FileSystem;
            protected string[] FileArray;
            protected string ContextPath;
            protected string DestinationPath;
            protected string TheTestFile;
            protected string FileToManipulate;
            protected string SourceFile;
            protected string DestFile;
            protected string DeleteFile;
            protected string TestDirectory;
            protected string[] DirectoryArray;

            public override void Context()
            {
                FileSystem = new DotNetFileSystem();
                ContextPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "infrastructure", "filesystem");
                DestinationPath = Path.Combine(ContextPath, "context");
                TheTestFile = Path.Combine(ContextPath, "Slipsum.txt");
                TestDirectory = Path.Combine(DestinationPath, "TestDirectory");
            }
        }

        public class when_doing_file_system_path_operations_with_DotNetFileSystem : DotNetFileSystemSpecsBase
        {
            public override void Because()
            {
            }

            [Fact]
            public void GetFullPath_should_return_the_full_path_to_an_item()
            {
                FileSystem.get_full_path("test.txt").ShouldEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt"));
            }

            [Fact]
            public void GetFileNameWithoutExtension_should_return_a_file_name_without_an_extension()
            {
                FileSystem.get_file_name_without_extension("test.txt").ShouldEqual("test");
            }

            [Fact]
            public void GetFileNameWithoutExtension_should_return_a_file_name_without_an_extension_even_with_a_full_path()
            {
                FileSystem.get_file_name_without_extension("C:\\temp\\test.txt").ShouldEqual("test");
            }

            [Fact]
            public void GetExtension_should_return_the_extension_of_the_filename()
            {
                FileSystem.get_file_extension("test.txt").ShouldEqual(".txt");
            }

            [Fact]
            public void GetExtension_should_return_the_extension_of_the_filename_even_with_a_full_path()
            {
                FileSystem.get_file_extension("C:\\temp\\test.txt").ShouldEqual(".txt");
            }

            [Fact]
            public void GetDirectoryName_should_return_the_directory_of_the_path_to_the_file()
            {
                FileSystem.get_directory_name("C:\\temp\\test.txt").ShouldEqual("C:\\temp");
            }

            [Fact]
            public void Combine_should_combine_the_file_paths_of_all_the_included_items_together()
            {
                FileSystem.combine_paths("C:\\temp", "yo", "filename.txt").ShouldEqual("C:\\temp\\yo\\filename.txt");
            }
        }

        [Category("Integration")]
        public class when_doing_file_system_operations_with_DotNetFileSystem : DotNetFileSystemSpecsBase
        {
            public override void Context()
            {
                base.Context();
                FileArray = new[]
                    {
                        Path.Combine(ContextPath, TheTestFile)
                    };

                DirectoryArray = new[]
                    {
                        DestinationPath
                    };
            }

            public override void Because()
            {
            }

            [Fact]
            public void GetFiles_should_return_string_array_of_files()
            {
                FileSystem.get_files(ContextPath, "*lipsum*", SearchOption.AllDirectories).ShouldEqual(FileArray);
            }

            [Fact]
            public void FileExists_should_return_true_if_file_exists()
            {
                FileSystem.file_exists(TheTestFile).ShouldBeTrue();
            }

            [Fact]
            public void FileExists_should_return_false_if_file_does_not_exists()
            {
                FileSystem.file_exists(Path.Combine(ContextPath, "IDontExist.txt")).ShouldBeFalse();
            }

            [Fact]
            public void DirectoryExists_should_return_true_if_directory_exists()
            {
                FileSystem.directory_exists(ContextPath).ShouldBeTrue();
            }

            [Fact]
            public void DirectoryExists_should_return_false_if_directory_does_not_exist()
            {
                FileSystem.directory_exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IDontExist")).ShouldBeFalse();
            }

            [Fact]
            public void GetFileSize_should_return_correct_file_size()
            {
                FileSystem.get_file_size(TheTestFile).ShouldEqual(5377);
            }

            [Fact]
            public void GetDirectories_should_return_a_string_array_with_directories()
            {
                FileSystem.get_directories(ContextPath).ShouldEqual(DirectoryArray);
            }
        }

        [Category("Integration")]
        public class when_running_FileMove_with_DotNetFileSystem : DotNetFileSystemSpecsBase
        {
            public override void Because()
            {
                SourceFile = Path.Combine(ContextPath, "MoveMe.txt");
                DestFile = Path.Combine(DestinationPath, "MoveMe.txt");
                if (!FileSystem.file_exists(SourceFile))
                {
                    File.Create(SourceFile);
                }
                if (FileSystem.file_exists(DestFile))
                {
                    File.Delete(DestFile);
                }
                FileSystem.move_file(SourceFile, DestFile);
            }

            [Fact]
            public void Move_me_text_file_should_not_exist_in_the_source_path()
            {
                FileSystem.file_exists(SourceFile).ShouldBeFalse();
            }

            [Fact]
            public void Move_me_text_file_should_exist_in_destination_path()
            {
                FileSystem.file_exists(DestFile).ShouldBeTrue();
            }
        }

        [Category("Integration")]
        public class when_running_FileCopy_with_DotNetFileSystem : DotNetFileSystemSpecsBase
        {
            public override void Because()
            {
                SourceFile = Path.Combine(ContextPath, "CopyMe.txt");
                DestFile = Path.Combine(DestinationPath, "CopyMe.txt");
                if (!FileSystem.file_exists(SourceFile))
                {
                    File.Create(SourceFile);
                }
                if (FileSystem.file_exists(DestFile))
                {
                    File.Delete(DestFile);
                }
                //Copy File
                FileSystem.copy_file(SourceFile, DestFile, true);
                //Overwrite File
                FileSystem.copy_file(SourceFile, DestFile, true);
            }

            [Fact]
            public void Copy_me_text_file_should_exist_in_context_path()
            {
                FileSystem.file_exists(SourceFile).ShouldBeTrue();
            }

            [Fact]
            public void Move_me_text_file_should_exist_in_destination_path()
            {
                FileSystem.file_exists(DestFile).ShouldBeTrue();
            }
        }

        [Category("Integration")]
        public class when_running_FileDelete_with_DotNetFileSystem : DotNetFileSystemSpecsBase
        {
            public override void Because()
            {
                DeleteFile = Path.Combine(DestinationPath, "DeleteMe.txt");
                if (!FileSystem.file_exists(DeleteFile))
                {
                    using (File.Create(DeleteFile))
                    {
                    }
                }

                FileSystem.delete_file(DeleteFile);
            }

            [Fact]
            public void delete_me_text_file_should_not_exist()
            {
                FileSystem.file_exists(DeleteFile).ShouldBeFalse();
            }
        }

        [Category("Integration")]
        public class when_running_CreateDirectory_with_DotNetFileSystem : DotNetFileSystemSpecsBase
        {
            public override void Because()
            {
                if (FileSystem.directory_exists(TestDirectory))
                {
                    Directory.Delete(TestDirectory);
                }

                FileSystem.create_directory(TestDirectory);
            }

            [Fact]
            public void test_directory_should_exist()
            {
                FileSystem.directory_exists(TestDirectory).ShouldBeTrue();
            }
        }

        [Category("Integration")]
        public class when_running_GetFileModDate_with_DotNetFileSystem : DotNetFileSystemSpecsBase
        {
            public override void Because()
            {
                File.SetCreationTime(TheTestFile, DateTime.Now.AddDays(-1));
                File.SetLastWriteTime(TheTestFile, DateTime.Now.AddDays(-1));
            }

            [Fact]
            public void should_have_correct_modified_date()
            {
                FileSystem.get_file_modified_date(TheTestFile).ToShortDateString().ShouldEqual(DateTime.Now.AddDays(-1).ToShortDateString());
            }
        }
    }
}