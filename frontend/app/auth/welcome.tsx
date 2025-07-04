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
  const handleRoleSelection = (role: 'volunteer' | 'organization') => {
    router.push(`/auth/login?role=${role}`);
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

        {/* Welcome text */}
        <View style={styles.welcomeContainer}>
          <Text style={styles.welcomeTitle}>Välkommen!</Text>
          <Text style={styles.welcomeSubtitle}>Vad vill du göra?</Text>
        </View>

        {/* Role selection buttons */}
        <View style={styles.roleContainer}>
          <TouchableOpacity
            style={styles.roleButton}
            onPress={() => handleRoleSelection('volunteer')}
          >
            <View style={styles.roleIcon}>
              <Text style={styles.roleIconText}>👤</Text>
            </View>
            <Text style={styles.roleTitle}>Volontär</Text>
            <Text style={styles.roleDescription}>Jag vill hjälpa andra</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={styles.roleButton}
            onPress={() => handleRoleSelection('organization')}
          >
            <View style={styles.roleIcon}>
              <Text style={styles.roleIconText}>🏢</Text>
            </View>
            <Text style={styles.roleTitle}>Organisation</Text>
            <Text style={styles.roleDescription}>Jag behöver hjälp</Text>
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
  logoPlaceholder: {
    width: Math.min(wp(30), 120),
    height: Math.min(wp(30), 120),
    borderRadius: Math.min(wp(15), 60),
    backgroundColor: Colors.ui.white,
    alignItems: 'center',
    justifyContent: 'center',
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  logoText: {
    fontSize: responsiveFontSize(40),
  },
  logoTitle: {
    fontSize: responsiveFontSize(28),
    fontWeight: 'bold',
    color: Colors.ui.white,
    marginTop: responsiveSpacing(12),
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
  welcomeSubtitle: {
    fontSize: responsiveFontSize(18),
    color: Colors.text.body,
    textAlign: 'center',
    marginTop: responsiveSpacing(8),
  },
  roleContainer: {
    width: '100%',
    alignItems: 'center',
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
