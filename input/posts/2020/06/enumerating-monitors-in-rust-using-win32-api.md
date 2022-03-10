---
Title: Enumerating monitors in Rust using Win32 API
Slug: enumerating-monitors-in-rust-using-win32-api
Date: 2020-06-08
RedirectFrom: 2020/06/enumerating-monitors-in-rust-using-win32-api/index.html
Tags:
- Rust
- Win32
---

I had to enumerate all monitors yesterday using the 
[EnumDisplayMonitors](https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaymonitors) 
Win32 function, and I thought I would write a couple of lines about what I did since I couldn't find any useful 
information about how to do this.

## Import the winapi crate

First, we need to import the [winapi crate](https://docs.rs/winapi/0.3.8/winapi/index.html).
We could of course define all Win32 [FFI](https://doc.rust-lang.org/nomicon/ffi.html) calls ourselves, 
but this saves us a lot of keystrokes and make our lives easier.

```toml
[dependencies]
winapi = { version = "0.3.6", features = ["winuser"] }
```

## Enumerating the monitors

With the winapi crate in place, we can start writing the code. I won't go into details about it,
but it should show a couple of things.

1. How to use unsafe code
2. How to define a callback from an FFI function
3. How to convert [WCHAR](https://docs.rs/winapi/0.3.8/winapi/um/winnt/type.WCHAR.html) 
   (a [u16](https://doc.rust-lang.org/std/u16/index.html) type alias)
   to [OsString](https://doc.rust-lang.org/std/ffi/struct.OsString.html)
4. How to convert from [OsString](https://doc.rust-lang.org/std/ffi/struct.OsString.html) 
   to [&str](https://doc.rust-lang.org/std/primitive.str.html)
5. How to cast to `*mut _` (a type placeholder) when we want Rust to figure out the type.

```rust
use std::ffi::OsString;
use std::io::Error;
use std::mem;
use std::ptr;
use std::os::windows::ffi::OsStringExt;

use winapi::shared::minwindef::{LPARAM, TRUE, BOOL};
use winapi::shared::windef::{HMONITOR, HDC, LPRECT, RECT};
use winapi::um::winuser::{EnumDisplayMonitors, GetMonitorInfoW, MONITORINFOEXW};

fn main() {
    for monitor in enumerate_monitors() {
        // Convert the WCHAR[] to a unicode OsString
        let name = match &monitor.szDevice[..].iter().position(|c| *c == 0) {
            Some(len) => OsString::from_wide(&monitor.szDevice[0..*len]),
            None => OsString::from_wide(&monitor.szDevice[0..monitor.szDevice.len()]),
        };

        // Print some information to the console
        println!("Display name = {}", name.to_str().unwrap());
        println!("    Left: {}", monitor.rcWork.left);
        println!("   Right: {}", monitor.rcWork.right);
        println!("     Top: {}", monitor.rcWork.top);
        println!("  Bottom: {}", monitor.rcWork.bottom);
    }
}

///////////////////////////////////////////////////////////////
// The method that numerates all monitors

fn enumerate_monitors() -> Vec<MONITORINFOEXW> {
    // Define the vector where we will store the result
    let mut monitors = Vec::<MONITORINFOEXW>::new();
    let userdata = &mut monitors as *mut _;

    let result = unsafe {
        EnumDisplayMonitors(
            ptr::null_mut(),
            ptr::null(),
            Some(enumerate_monitors_callback),
            userdata as LPARAM,
        )
    };

    if result != TRUE {
        // Get the last error for the current thread.
        // This is analogous to calling the Win32 API GetLastError.
        panic!("Could not enumerate monitors: {}", Error::last_os_error());
    }

    monitors
}

///////////////////////////////////////////////////////////////
// The callback from EnumDisplayMonitors

unsafe extern "system" fn enumerate_monitors_callback(
    monitor: HMONITOR,
    _: HDC,
    _: LPRECT,
    userdata: LPARAM,
) -> BOOL {
    // Get the userdata where we will store the result
    let monitors: &mut Vec<MONITORINFOEXW> = mem::transmute(userdata);

    // Initialize the MONITORINFOEXW structure and get a pointer to it
    let mut monitor_info: MONITORINFOEXW = mem::zeroed();
    monitor_info.cbSize = mem::size_of::<MONITORINFOEXW>() as u32;
    let monitor_info_ptr = <*mut _>::cast(&mut monitor_info);

    // Call the GetMonitorInfoW win32 API
    let result = GetMonitorInfoW(monitor, monitor_info_ptr);
    if result == TRUE {
        // Push the information we received to userdata
        monitors.push(monitor_info);
    }

    TRUE
}
```