# Changelog
All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0-pre.4] - 2022-06-21
### Fixed
 - Fixed that UICullingBehaviour causes a NullReferenceException, when the Component gets enabled without the Rect and/or Viewport being assigned already. Now it returns "not visible" for this case and no longer causes an exception.

## [1.0.0-pre.3] - 2022-05-05
### Fixed
 - Fixed that ```UICullingBehaviour.isVisible``` returns ```true``` when the rect/viewport width or height is 0.

## [1.0.0-pre.2] - 2022-05-05
### Fixed
 - Fixed compile error when building a Player

## [1.0.0-pre.1] - 2022-04-23
### Added
 - Public release
