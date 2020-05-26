---
Title: Targeting ARM64 for Windows in Rust
Slug: targeting-arm-for-windows-in-rust
Published: 2020-05-26
Tags:
- Rust
- ARM64
- Windows
---

I wanted to run a thing I'm building with Rust on my Surface Pro X which is an 
ARM64 device the other day. My initial thought when I got the idea, was 
"I hope it's not complicated to do".

TLDR; it wasn't.

<!--excerpt-->

## Toolchain targets

One thing to be aware of is that ARM64, or `aarch64-pc-windows-msvc` as the toolchain
target is known as, is that it belongs in `tier 2` of Rust's platform support.

While tier 1 is "_Guaranteed to run_", tier 2 is only "_Guaranteed to build_".

> Tier 2 platforms can be thought of as "guaranteed to build". Automated tests 
are not run, so it's not guaranteed to produce a working build, but platforms often work to quite a good degree [...]

So what do the different parts in the toolchain target name mean?

<table>
  <tbody>
  <tr>
    <td><code>aarch64</code></td>
    <td>This is the platform architecture we're targeting, in this case, ARM64.</td>
  </tr>
  <tr>
    <td><code>pc-windows</code></td>
    <td>This is the operating system we're targeting, in this case, Windows.</td>
  </tr>
  <tr>
    <td><code>msvc</code></td>
    <td>This is the compiler we want to use. MSVC is an abbreviation of "Microsoft Visual C++".</td>
  </tr>
  </tbody>
</table>

If you're interested in reading more about the different supported platforms
in Rust, you can find the full list over at  
https://forge.rust-lang.org/release/platform-support.html.

## Installing the toolchain target

Rust supports cross-compilation, so there's no need to build our ARM64 app on an 
ARM64 machine, and even if we wanted to, it would prove difficult since essential 
tools like [Cargo](https://doc.rust-lang.org/cargo/) isn't available for ARM64 on 
Windows. The only prerequisite for cross-compiling is a compiler 
that can target the desired platform and all the necessary dependencies 
required to compile the code.

To compile our application for ARM64, we need to install the 
`aarch64-pc-windows-msvc` target for our Rust compiler toolchain.

```text
> rustup target install aarch64-pc-windows-msvc
```

If you want to see what toolchain targets are available or installed, you can 
use the `rustup target list` command.

```
> rustup target list
aarch64-apple-ios
aarch64-fuchsia
aarch64-linux-android
aarch64-pc-windows-msvc (installed)
aarch64-unknown-linux-gnu
```

## Building for ARM64

Now when the correct toolchain target has been installed, let's go ahead and 
create a new binary and build it with our newly installed target.

```text
> cargo build --target=aarch64-pc-windows-msvc
   Compiling helloworld v0.1.0 (D:\Source\local\helloworld)
    Finished dev [unoptimized + debuginfo] target(s) in 0.36s
```

### Automatically build with a specific target

If you always want to build for this target, you can create`.cargo/config` 
in your repository root and add the following contents to it:

```toml
[build]
target = "aarch64-pc-windows-msvc"
```

## Running on an ARM64 machine

Everything compiled! Not a big surprise, it's "Guaranteed to build" after all,
but it felt good anyway. Time to test it out on an actual ARM64 machine.

```text
> ./helloworld.exe
-1073741515
```

### Oh noes

The application exited with the error code `-1073741515`, 
more famously known as `0xC0000135`, which is Windows' way of telling us that 
an essential component is missing. It turns out that we need to install the 
[_Microsoft Visual C++ Redistributable for Visual Studio 2019_](https://aka.ms/vs/16/release/VC_redist.arm64.exe)
for ARM64.

Installing the MSVC redistributable and rerunning our app 
now produces the expected output.

```text
> ./helloworld.exe
Hello World
```

## Wrapping up

Not super complicated stuff, but hopefully this post has been useful for you if you
wanted to learn more about Rust's different platform tiers, toolchain targets, and cross-compilation.