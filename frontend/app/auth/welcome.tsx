import { LinearGradient } from 'expo-linear-gradient';
import { router } from 'expo-router';
import React from 'react';
import {
  StyleSheet,
  Text,
  TouchableOpacity,
  View
} from 'react-native';

import { VolontarLogo } from '@/components/VolontarLogo';
import { Colors } from '@/constants/Colors';
import { hp, responsiveFontSize, responsiveSpacing, wp } from '@/utils/responsive';

export default function WelcomeScreen() {
  const handleGoToRegister = () => {
    router.push('/auth/login');
  };

  return (
    <LinearGradient
      colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
      style={styles.container}
    >
      <View style={styles.content}>
        {/* Logo */}
        <View style={styles.logoContainer}>
          <VolontarLogo size={Math.min(wp(25), 160)} showText={true} />
        </View>

        {/* Welcome title only */}
        <View style={styles.welcomeContainer}>
          <Text style={styles.welcomeTitle}>Välkommen!</Text>
        </View>

        {/* Single user creation button */}
        <View style={styles.roleContainer}>
          <TouchableOpacity
            style={styles.roleButton}
            onPress={handleGoToRegister}
          >
            <View style={styles.roleIcon}>
              <Text style={styles.roleIconText}>👤</Text>
            </View>
            <Text style={styles.roleTitle}>Användare</Text>
          </TouchableOpacity>
        </View>

        {/* Login link */}
        <TouchableOpacity
          style={styles.loginLink}
          onPress={() => router.push('/auth/login')}
        >
          <Text style={styles.loginLinkText}>Har du redan ett konto? Logga in</Text>
        </TouchableOpacity>
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
  logoContainer: {
    alignItems: 'center',
    marginTop: hp(5),
  },
  welcomeContainer: {
    alignItems: 'center',
    marginTop: hp(3),
  },
  welcomeTitle: {
    fontSize: responsiveFontSize(28),
    fontWeight: 'bold',
    color: Colors.text.heading,
    textAlign: 'center',
  },
  roleContainer: {
    width: '100%',
    alignItems: 'center',
    marginTop: hp(4),
    gap: responsiveSpacing(20),
  },
  roleButton: {
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    paddingVertical: responsiveSpacing(24),
    paddingHorizontal: wp(8),
    width: '100%',
    maxWidth: 300,
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
  roleIcon: {
    width: 60,
    height: 60,
    borderRadius: 30,
    backgroundColor: Colors.gradient.start,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: responsiveSpacing(12),
  },
  roleIconText: {
    fontSize: responsiveFontSize(24),
  },
  roleTitle: {
    fontSize: responsiveFontSize(20),
    fontWeight: 'bold',
    color: Colors.ui.black,
    marginBottom: responsiveSpacing(4),
  },
  roleDescription: {
    fontSize: responsiveFontSize(14),
    color: Colors.ui.gray,
    textAlign: 'center',
  },
  loginLink: {
    paddingVertical: responsiveSpacing(12),
    marginBottom: hp(2),
  },
  loginLinkText: {
    fontSize: responsiveFontSize(16),
    color: Colors.ui.white,
    textAlign: 'center',
    textDecorationLine: 'underline',
  },
});
