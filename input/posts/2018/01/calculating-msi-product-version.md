---
Title: Calculating product versions for MSI packages compatible with semantic versioning
Slug: calculating-msi-product-version
Date: 2018-01-30
RedirectFrom: 2018/01/calculating-msi-product-version/index.html
Tags:
- MSI
- WiX
---

In my new project [Jarvis](https://github.com/spectresystems/jarvis) I wanted to start
generate preview versions of the MSI packages, but one problem with that is that MSI
requires the product version to be in the format `Major.Minor.Patch` which isn't compatible with [semantic verisoning](https://semver.org). We CAN use the `Major.Minor.Patch.Revision` format as a product version, but that won't work with [major upgrades](https://support.firegiant.com/hc/en-us/articles/230912187-Implement-major-upgrade-). An example of this would be `1.2.3-alpha45` which would require a different version number than `1.2.3-alpha46`.

A solution to this is to use the following formula (originally found [here](https://github.com/semver/semver/issues/332)), where `PRE` is the pre-release number (normally the number of commits for the current patch).

```
ENSURE (PATCH >= 0 && PATCH < 54)
ENSURE (PRE >= 0 && PRE < 1000)

PATCH = 10000 + (PATCH * 1000) + PRE
```

Which would result in the following versions.

Semantic version | Patch | Pre-release | MSI version
---------------- | ----- | ----------- | -----------
1.2.0-alpha1     | 0     | 1           | 1.2.10001
1.2.0-alpha2     | 0     | 2           | 1.2.10002
1.2.0            | 0     | 3           | 1.2.10003
1.2.3-alpha1     | 3     | 1           | 1.2.40001
1.2.3            | 3     | 2           | 1.2.40002
1.2.53-alpha32   | 53    | 32          | 1.2.63032
1.2.53           | 53    | 33          | 1.2.63033

This is not by any means fool-proof but might act like a good starting point.