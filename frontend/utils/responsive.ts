import { Dimensions } from 'react-native';

// Get screen dimensions
export const getScreenDimensions = () => {
  const { width, height } = Dimensions.get('window');
  return { width, height };
};

// Responsive breakpoints
export const breakpoints = {
  small: 400,
  medium: 768,
  large: 1024,
};

// Check screen size
export const isSmallScreen = () => {
  const { width } = getScreenDimensions();
  return width < breakpoints.small;
};

export const isMediumScreen = () => {
  const { width } = getScreenDimensions();
  return width >= breakpoints.small && width < breakpoints.medium;
};

export const isLargeScreen = () => {
  const { width } = getScreenDimensions();
  return width >= breakpoints.medium;
};

// Responsive font sizes - desktop optimized with moderate scaling
export const responsiveFontSize = (baseSize: number) => {
  const { width } = getScreenDimensions();
  
  // More conservative scaling for better desktop experience
  if (width < breakpoints.small) {
    // Mobile: use original scaling
    const scale = width / 375;
    const limitedScale = Math.min(Math.max(scale, 0.9), 1.1);
    return Math.round(baseSize * limitedScale);
  } else if (width < breakpoints.medium) {
    // Tablet: slight increase
    return Math.round(baseSize * 1.15);
  } else {
    // Desktop: moderate increase
    return Math.round(baseSize * 1.3);
  }
};

// Responsive spacing - desktop optimized with moderate scaling
export const responsiveSpacing = (baseSpacing: number) => {
  const { width } = getScreenDimensions();
  
  // More conservative scaling for spacing
  if (width < breakpoints.small) {
    // Mobile: use original scaling
    const scale = width / 375;
    const limitedScale = Math.min(Math.max(scale, 0.9), 1.1);
    return Math.round(baseSpacing * limitedScale);
  } else if (width < breakpoints.medium) {
    // Tablet: slight increase
    return Math.round(baseSpacing * 1.1);
  } else {
    // Desktop: moderate increase
    return Math.round(baseSpacing * 1.2);
  }
};

// Get responsive width percentage
export const wp = (percentage: number) => {
  const { width } = getScreenDimensions();
  return (width * percentage) / 100;
};

// Get responsive height percentage
export const hp = (percentage: number) => {
  const { height } = getScreenDimensions();
  return (height * percentage) / 100;
};

// Check if device is in landscape mode
export const isLandscape = () => {
  const { width, height } = getScreenDimensions();
  return width > height;
};

// Get number of columns for grid based on screen size
export const getGridColumns = () => {
  const { width } = getScreenDimensions();
  if (width < 500) return 1;
  if (width < 800) return 2;
  return 3;
};

// Get desktop-optimized container width
export const getContainerWidth = (maxWidth: number = 700) => {
  const { width } = getScreenDimensions();
  return Math.min(width * 0.9, maxWidth);
};

// Get desktop-optimized content padding
export const getContentPadding = () => {
  const { width } = getScreenDimensions();
  if (width < breakpoints.small) return wp(8); // Mobile: 8% of screen width
  if (width < breakpoints.medium) return 32; // Tablet: fixed 32px
  return 48; // Desktop: fixed 48px
};
