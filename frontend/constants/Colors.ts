/**
 * Volontar App Colors - Based on Figma Design
 * Orange/Yellow gradient with Blue accents
 */

export const Colors = {
  // Primary Volontar Colors from Figma (Exact Match)
  primary: {
    orange: '#FF8C42',      // Exact orange from Figma
    orangeLight: '#FFD4A3', // Very light beige/orange from Figma
    orangeDark: '#E8743A',  // Darker orange
    blue: '#1E3A8A',        // Deep blue from Figma headers
    blueLight: '#3B82F6',   // Lighter blue
    blueDark: '#1E40AF',    // Darker blue
  },
  
  // Gradient Colors (Exact Figma Match)
  gradient: {
    start: '#FFF6EA',    // Väldigt ljus creme
    middle: '#FFEDD5',   // Mittpunkt
    end: '#FFDAA9',      // Mjuk aprikos
  },
  
  // UI Colors
  ui: {
    white: '#FFFFFF',
    black: '#000000',
    gray: '#757575',
    lightGray: '#E0E0E0',
    darkGray: '#424242',
    background: '#F5F5F5',
    card: '#FFFFFF',
    shadow: 'rgba(0, 0, 0, 0.1)',
  },
  
  // Typography Colors (Figma Spec)
  text: {
    heading: '#1E3A8A',     // Mörkblå för rubriker
    body: '#424242',        // Mörkgrå för brödtext
    highlight: '#FF8C42',   // Orange för highlights
    placeholder: '#757575', // Grå för placeholder
  },
  
  // Status Colors
  status: {
    success: '#4CAF50',
    warning: '#FF9800',
    error: '#F44336',
    info: '#2196F3',
  },
  
  // Legacy support for existing components
  light: {
    text: '#000000',
    background: '#FFFFFF',
    tint: '#1976D2',
    icon: '#757575',
    tabIconDefault: '#757575',
    tabIconSelected: '#1976D2',
  },
  dark: {
    text: '#FFFFFF',
    background: '#000000',
    tint: '#42A5F5',
    icon: '#9BA1A6',
    tabIconDefault: '#9BA1A6',
    tabIconSelected: '#42A5F5',
  },
};
