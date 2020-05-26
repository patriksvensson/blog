---
Title: Targeting ARM64 for Windows in Rust
Slug: targeting-arm-for-windows-in-rust
Published: 2020-05-26
Draft: True
Tags:
- Rust
- ARM64
- Windows
---

I wanted to run a thing I'm building with Rust on my Surface Pro X which is
an ARM64 device the other day my initial thought when I got the idea was 
_"that can't be very difficult to do"_.

TLDR; it wasn't.

<!--excerpt-->

# Toolchain targets

One thing to know is that ARM64, or `aarch64-pc-windows-msvc` as the toolchain
target is known as, is that it belongs in `tier 2` of Rust's platform support.

While tier 1 is "_Guaranteed to run_", tier 2 is only "_Guaranteed to build_".

> Tier 2 platforms can be thought of as "guaranteed to build". Automated tests are not run so it's not guaranteed to produce a working build, but platforms often work to quite a good degree [...]

So what does the different parts in the toolchain target name mean?

* `arch64`
  The platform architecture we're targeting, in this case ARM64.
* `pc-windows`
  The operating system we're targeting, in this case Windows.
* `msvc`
  The compiler we're using. MSVC is an abbreviation of "Microsoft Visual C++"

If you're interested in reading more about the different supported platforms
in Rust, you can find the full list over at  
https://forge.rust-lang.org/release/platform-support.html.

# Installing the toolchain target

Rust supports cross compilation, so there's no need to actully build our ARM64 app
on an ARM64 machine, and even if we wanted to, it would prove difficult since 
essential tools like [Cargo](https://doc.rust-lang.org/cargo/) isn't available 
for ARM64 on Windows. The only prerequisite for cross compiling is that you need
a compiler that can target the platform installed on you machine (in our case Microsoft Visual C++) 
and all the necessary dependencies that's required to compile our code.

To compile our application for ARM64 we need to install the `aarch64-pc-windows-msvc` 
target for our Rust compiler toolchain.

```text
> rustup target install aarch64-pc-windows-msvc
```

If you want to see what toolchain targets are availabale or installed, you can 
use the `rustup target list` command.

```
> rustup target list
aarch64-apple-ios
aarch64-fuchsia
aarch64-linux-android
aarch64-pc-windows-msvc (installed)
aarch64-unknown-linux-gnu
```

# Building for ARM64

Now when the correct toolchain target is installed, let's go ahead and
create a new binary called _armtest_ and build it.

```text
> cargo new armtest
> cd armtest
> cargo build --target=aarch64-pc-windows-msvc
   Compiling armtest v0.1.0 (D:\Source\local\armtest)
    Finished dev [unoptimized + debuginfo] target(s) in 0.36s
```

# Running on an ARM64 machine

Everything compiled! Not a big surprise, it's "Guaranteed to build" after all,
but it felt good anyway.  
Time to test it out on an actual ARM-64 machine.

```text
> ./armtest.exe
-1073741515
```

## Oh noes

The application exited with the error code `-1073741515`, more famously known as `0xC0000135`, 
which is Windows way of telling us that an essential component is missing. 
Turns out that we need to install the 
[_Microsoft Visual C++ Redistributable for Visual Studio 2019_](https://aka.ms/vs/16/release/VC_redist.arm64.exe)
for ARM64.

Installing that and running it again produces the expected output.

```text
> ./armtest.exe
Hello World
```

# Wrapping up

Not super complicated stuff, but hopefully this post have been useful for you if you
wanted to learn more about Rust's different platform tiers, toolchain targets and cross compilation.