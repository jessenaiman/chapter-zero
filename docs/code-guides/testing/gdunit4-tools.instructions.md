---
layout: default
title: Tools and Helpers
parent: Advanced Testing
nav_order: 1
---

# Tools and Helpers

GdUnit provides several tools to help you write tests, including managing objects to free and accessing a temporary
    filesystem to store your test data.

---

## auto_free()

A little helper for automatically releasing the objects you create after test execution.

*Note that objects do not manage memory. If a class inherits from Object, you will need to delete its instances manually.
References keep an internal reference counter so that they are automatically released when no longer in use,
and only then. Therefore, you do not need to free references manually with Object.free().*

Use **auto_free()** to automatically release objects when they are no longer in use.
Objects that are covered by **auto_free** only live in the scope where they are used.
These scopes include the test suite setup, test case setup, and the tests themselves.


```cs
public static T AutoFree<T>(T obj);
```

```cs
using static Assertions;

  ...
    // using AutoFree() on Path object to register for freeing after the test is finished
    AssertObject(AutoFree(new Godot.Path())).IsNotInstanceOf<Godot.Tree>();
```

```cs
public static string CreateTempDir(string path);
```

Here’s an example:

```ruby
func test_save_game_data():
    # create a temporay directory to store test data
    var temp_dir := create_temp_dir("examples/game/save")
    var file_to_save := temp_dir + "/save_game.dat"

    var data = {
        'user': "Hoschi",
        'level': 42
    }
    var file := File.new()
    file.open(file_to_save, File.WRITE)
    file.store_line(JSON.print(data))
    file.close()

        # the data is saved at "user://tmp/examples/game/save/save_game.dat"
    assert_bool(file.file_exists(file_to_save)).is_true()
```

## clean_temp_dir()

Deletes the temporary directory created by **create_temp_dir()**.
This function is called automatically after each execution of the test suite to ensure that the temporary directory
is clean and ready for the next test suite.

{% tabs tools-clean_temp_dir %}
{% tab tools-clean_temp_dir GdScript %}

```ruby
func clean_temp_dir():
```

{% endtab %}
{% tab tools-clean_temp_dir C# %}

```cs
public static void ClearTempDir();
```

{% endtab %}
{% endtabs %}

## create_temp_file()

The **create_temp_file()** function creates a new File under the temporary directory *user://tmp* + *\<relative_path\>*
with the given name *\<file_name\>* and file *\<mode\>* (default = File.WRITE).<br>
If successful, the returned File is automatically closed after the execution of the test suite.<br>
We can create a small test file at the beginning of a test suite in the **before()** function and read it later
in the test. It is not necessary to close the file, as the GdUnit test runner will close it automatically.

```ruby
func create_temp_file(relative_path :String, file_name :String, mode :=File.WRITE) -> File:
```

Here’s an example:

```ruby
# setup test data
func before():
    # opens a tmp file with WRITE mode under "user://tmp/examples/game/game.sav" (auto closed)
    var file := create_temp_file("examples/game", "game.sav")
    assert_object(file).is_not_null()
    # write some example data
    file.store_line("some data")
    # not needs to be manually close, will be auto closed before test execution

func test_create_temp_file():
    # opens the stored tmp file with READ mode under "user://tmp/examples/game/game.sav" (auto closed)
    var file_read := create_temp_file("examples/game", "game.sav", File.READ)
    assert_object(file_read).is_not_null()
    assert_str(file_read.get_as_text()).is_equal("some data\n")
    # not needs to be manually close, will be auto closed after test suite execution
```

## resource_as_array()

This function reads a resource file located at the given path *\<resource_path\>* and returns its contents as a PackedStringArray.

```ruby
func resource_as_array(resource_path :String) -> PoolStringArray:
```

Here’s an example:

```ruby
var rows = ["row a", "row b"]
var file_content := resource_as_array("res://myproject/test/resources/TestRows.txt")
assert_array(rows).contains_exactly(file_content)
```

## resource_as_string()

This function reads a resource file located at the given path \<resource_path\> and returned the content as String.

```ruby
func resource_as_string(resource_path :String) -> String:
```

Here’s an example:

```ruby
var rows = "row a\nrow b\n"
var file_content := resource_as_string("res://myproject/test/resources/TestRows.txt")
assert_string(rows).is_equal(file_content)
```
