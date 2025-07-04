import { LinearGradient } from 'expo-linear-gradient';
import { router } from 'expo-router';
import React, { useState } from 'react';
import {
  Dimensions,
  StyleSheet,
  Text,
  TouchableOpacity,
  View
} from 'react-native';

import { VolontarLogo } from '@/components/VolontarLogo';
import { Colors } from '@/constants/Colors';
import { ONBOARDING_DATA } from '@/constants/onboarding';
import { hp, responsiveFontSize, responsiveSpacing, wp } from '@/utils/responsive';

const { width, height } = Dimensions.get('window');

export default function OnboardingScreen() {
  const [currentIndex, setCurrentIndex] = useState(0);

  const handleNext = () => {
    if (currentIndex < ONBOARDING_DATA.length - 1) {
      setCurrentIndex(currentIndex + 1);
    } else {
      // Navigate to auth screen
      router.replace('/auth/welcome');
    }
  };

  const handleSkip = () => {
    router.replace('/auth/welcome');
  };

  const currentData = ONBOARDING_DATA[currentIndex];

  return (
    <LinearGradient
      colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
      style={styles.container}
    >
      <View style={styles.content}>
        {/* Skip button */}
        <TouchableOpacity style={styles.skipButton} onPress={handleSkip}>
          <Text style={styles.skipText}>Hoppa över</Text>
        </TouchableOpacity>

        {/* Logo */}
        <View style={styles.logoContainer}>
          <VolontarLogo size={Math.min(wp(40), 160)} showText={false} />
        </View>

        {/* Title */}
        <Text style={styles.title}>{currentData.title}</Text>

        {/* Subtitle */}
        <Text style={styles.subtitle}>{currentData.subtitle}</Text>

        {/* Description */}
        <Text style={styles.description}>{currentData.description}</Text>

        {/* Page indicators */}
        <View style={styles.indicatorContainer}>
          {ONBOARDING_DATA.map((_, index: number) => (
            <View
              key={index}
              style={[
                styles.indicator,
                index === currentIndex && styles.activeIndicator,
              ]}
            />
          ))}
        </View>

        {/* Buttons - Two buttons on first screen, one on others */}
        {currentIndex === 0 ? (
          <View style={styles.buttonContainer}>
            <TouchableOpacity style={styles.primaryButton} onPress={() => router.replace('/auth/welcome')}>
              <Text style={styles.primaryButtonText}>Kom igång</Text>
            </TouchableOpacity>
            <TouchableOpacity style={styles.secondaryButton} onPress={() => router.replace('/auth/login')}>
              <Text style={styles.secondaryButtonText}>Jag har redan ett konto</Text>
            </TouchableOpacity>
          </View>
        ) : (
          <TouchableOpacity style={styles.nextButton} onPress={handleNext}>
            <Text style={styles.nextButtonText}>
              {currentIndex === ONBOARDING_DATA.length - 1 ? 'Kom igång' : 'Nästa'}
            </Text>
          </TouchableOpacity>
        )}
      </View>
    </LinearGradient>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  content: {
    flex: 1,
    paddingHorizontal: wp(8),
    paddingVertical: hp(5),
    alignItems: 'center',
    justifyContent: 'center',
    maxWidth: 700,
    alignSelf: 'center',
    width: '100%',
  },
  skipButton: {
    alignSelf: 'flex-end',
    paddingVertical: responsiveSpacing(8),
    paddingHorizontal: responsiveSpacing(16),
  },
  skipText: {
    fontSize: responsiveFontSize(16),
    color: Colors.ui.white,
    fontWeight: '500',
  },
  logoContainer: {
    alignItems: 'center',
    marginTop: hp(5),
  },
  title: {
    fontSize: responsiveFontSize(32),
    fontWeight: 'bold',
    color: Colors.text.heading,
    textAlign: 'center',
    marginTop: hp(4),
  },
  subtitle: {
    fontSize: responsiveFontSize(20),
    fontWeight: '600',
    color: Colors.text.heading,
    textAlign: 'center',
    marginTop: responsiveSpacing(8),
  },
  description: {
    fontSize: responsiveFontSize(16),
    color: Colors.text.body,
    textAlign: 'center',
    lineHeight: responsiveFontSize(24),
    paddingHorizontal: wp(5),
    marginTop: responsiveSpacing(16),
  },
  indicatorContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: hp(4),
  },
  indicator: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: 'rgba(255, 255, 255, 0.4)',
    marginHorizontal: 4,
  },
  activeIndicator: {
    backgroundColor: Colors.ui.white,
    width: 24,
  },
  nextButton: {
    backgroundColor: Colors.primary.blue,
    paddingVertical: responsiveSpacing(16),
    paddingHorizontal: wp(20),
    borderRadius: 20,
    minWidth: wp(60),
    alignItems: 'center',
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
    marginBottom: hp(3),
  },
  nextButtonText: {
    fontSize: responsiveFontSize(18),
    fontWeight: '600',
    color: Colors.ui.white,
  },
  buttonContainer: {
    width: '100%',
    alignItems: 'center',
    gap: responsiveSpacing(12),
    marginBottom: hp(3),
  },
  primaryButton: {
    backgroundColor: Colors.primary.blue,
    paddingVertical: responsiveSpacing(16),
    paddingHorizontal: wp(20),
    borderRadius: 20,
    minWidth: wp(60),
    alignItems: 'center',
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  primaryButtonText: {
    fontSize: responsiveFontSize(18),
    fontWeight: '600',
    color: Colors.ui.white,
  },
  secondaryButton: {
    backgroundColor: Colors.primary.orange,
    paddingVertical: responsiveSpacing(16),
    paddingHorizontal: wp(20),
    borderRadius: 20,
    minWidth: wp(60),
    alignItems: 'center',
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  secondaryButtonText: {
    fontSize: responsiveFontSize(18),
    fontWeight: '600',
    color: Colors.ui.white,
  },
});
