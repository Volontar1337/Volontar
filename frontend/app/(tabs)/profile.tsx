import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import React from 'react';
import { ScrollView, StyleSheet, Text, TouchableOpacity, View } from 'react-native';

import { Colors } from '@/constants/Colors';
import { useAuth } from '@/contexts/AuthContext';
import { hp, responsiveFontSize, responsiveSpacing, wp } from '@/utils/responsive';

export default function ProfileScreen() {
  const { user, logout } = useAuth();

  // Check if user is organization
  const isOrganization = user?.role === 'organization';

  const functionCards = [
    { id: 1, title: 'Meddelanden', icon: 'mail-outline', color: Colors.text.heading },
    { id: 2, title: 'Bokmärken', icon: 'bookmark-outline', color: Colors.text.heading },
    { id: 3, title: 'Påminnelser', icon: 'notifications-outline', color: Colors.text.heading },
    { id: 4, title: 'Avslutade uppdrag', icon: 'checkmark-circle-outline', color: Colors.text.heading },
    { id: 5, title: 'Mina uppdrag', icon: 'people-outline', color: Colors.text.heading },
    { id: 6, title: 'Favoriter', icon: 'heart-outline', color: Colors.text.heading },
  ];

  return (
    <LinearGradient
      colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
      style={styles.container}
    >
      {/* Header with gradient background */}
      <LinearGradient
        colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
        style={styles.header}
      >
        <View style={styles.headerContent}>
          <Text style={styles.headerTitle}>Min Sida</Text>
          
          {/* User Profile Section */}
          <View style={styles.userSection}>
            <View style={styles.profileImageContainer}>
              <View style={styles.profileImage}>
                <Text style={styles.profileInitials}>
                  {user?.firstName?.charAt(0)}{user?.lastName?.charAt(0)}
                </Text>
                <View style={styles.onlineIndicator} />
              </View>
            </View>
            
            <View style={styles.userInfo}>
              <Text style={styles.userName}>
                {user?.firstName} {user?.lastName}
              </Text>
              <View style={styles.statusContainer}>
                <Ionicons name="checkmark-circle" size={16} color={Colors.status.success} />
                <Text style={styles.statusText}>Öppen för nya uppdrag</Text>
              </View>
              <View style={styles.ratingContainer}>
                <View style={styles.starsContainer}>
                  {[1, 2, 3, 4, 5].map((star) => (
                    <Ionicons key={star} name="star" size={16} color={Colors.primary.orange} />
                  ))}
                </View>
                <Text style={styles.ratingText}>5.0</Text>
              </View>
              <Text style={styles.completedTasks}>✓ Avslutade uppdrag: 8/8</Text>
            </View>
          </View>
        </View>
      </LinearGradient>

      {/* Content Area */}
      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        {isOrganization ? (
          /* Organization Profile View */
          <>
            {/* Organization Stats */}
            <View style={styles.organizationStats}>
              <View style={styles.statItem}>
                <Text style={styles.statNumber}>12</Text>
                <Text style={styles.statLabel}>Pågående uppdrag</Text>
              </View>
              <View style={styles.statItem}>
                <Text style={styles.statNumber}>45</Text>
                <Text style={styles.statLabel}>Avslutade uppdrag</Text>
              </View>
            </View>

            {/* Organization Presentation Card */}
            <View style={styles.presentationCard}>
              <Text style={styles.presentationTitle}>Om oss</Text>
              <Text style={styles.presentationText}>
                Vi är en ideell organisation som arbetar för att hjälpa äldre och utsatta i vårt samhälle. 
                Vårt mål är att skapa en tryggare och mer omtänksam miljö för alla.
              </Text>
            </View>

            {/* Send Message Button */}
            <TouchableOpacity style={styles.messageButton}>
              <Ionicons name="mail-outline" size={20} color={Colors.ui.white} />
              <Text style={styles.messageButtonText}>Skicka meddelande</Text>
            </TouchableOpacity>
          </>
        ) : (
          /* Volunteer Profile View */
          <View style={styles.cardsGrid}>
            {functionCards.map((card) => (
              <TouchableOpacity key={card.id} style={styles.functionCard}>
                <Ionicons name={card.icon as any} size={24} color={card.color} />
                <Text style={styles.cardTitle}>{card.title}</Text>
              </TouchableOpacity>
            ))}
          </View>
        )}

        {/* Logout Button */}
        <TouchableOpacity style={styles.logoutButton} onPress={logout}>
          <Text style={styles.logoutButtonText}>Logga ut</Text>
        </TouchableOpacity>
      </ScrollView>

      {/* Bottom Navigation Space */}
      <View style={styles.bottomSpace} />
    </LinearGradient>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  header: {
    paddingTop: hp(6),
    paddingBottom: hp(3),
    paddingHorizontal: wp(5),
  },
  headerContent: {
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: responsiveFontSize(24),
    fontWeight: 'bold',
    color: Colors.text.heading,
    marginBottom: hp(2),
    textAlign: 'center',
  },
  userSection: {
    flexDirection: 'row',
    alignItems: 'center',
    width: '100%',
    justifyContent: 'center',
  },
  profileImageContainer: {
    position: 'relative',
    marginRight: responsiveSpacing(16),
  },
  profileImage: {
    width: 90,
    height: 90,
    borderRadius: 45,
    backgroundColor: Colors.ui.white,
    borderWidth: 2,
    borderColor: Colors.text.heading,
    alignItems: 'center',
    justifyContent: 'center',
    position: 'relative',
  },
  profileInitials: {
    fontSize: responsiveFontSize(28),
    fontWeight: 'bold',
    color: Colors.text.heading,
  },
  onlineIndicator: {
    position: 'absolute',
    bottom: 2,
    right: 2,
    width: 16,
    height: 16,
    borderRadius: 8,
    backgroundColor: Colors.status.success,
    borderWidth: 2,
    borderColor: Colors.ui.white,
  },
  userInfo: {
    flex: 1,
    alignItems: 'center',
  },
  userName: {
    fontSize: responsiveFontSize(20),
    fontWeight: 'bold',
    color: Colors.text.heading,
    marginBottom: responsiveSpacing(8),
    textAlign: 'center',
  },
  statusContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: responsiveSpacing(8),
  },
  statusText: {
    fontSize: responsiveFontSize(14),
    color: Colors.status.success,
    fontWeight: '600',
    marginLeft: responsiveSpacing(6),
  },
  ratingContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: responsiveSpacing(8),
  },
  starsContainer: {
    flexDirection: 'row',
    marginRight: responsiveSpacing(8),
  },
  ratingText: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.heading,
    fontWeight: '600',
  },
  completedTasks: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    textAlign: 'center',
  },
  content: {
    flex: 1,
    paddingHorizontal: wp(5),
    paddingTop: hp(3),
  },
  cardsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
    marginBottom: hp(3),
  },
  functionCard: {
    width: '48%',
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    padding: responsiveSpacing(20),
    alignItems: 'center',
    marginBottom: responsiveSpacing(16),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  cardTitle: {
    fontSize: responsiveFontSize(14),
    fontWeight: '600',
    color: Colors.text.heading,
    textAlign: 'center',
    marginTop: responsiveSpacing(12),
  },
  logoutButton: {
    backgroundColor: Colors.status.error,
    borderRadius: 20,
    paddingVertical: responsiveSpacing(16),
    alignItems: 'center',
    marginTop: hp(2),
    marginBottom: hp(3),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 4,
  },
  logoutButtonText: {
    fontSize: responsiveFontSize(16),
    fontWeight: '600',
    color: Colors.ui.white,
  },
  bottomSpace: {
    height: hp(10), // Space for tab bar
  },
  // Organization Profile Styles
  organizationStats: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    padding: responsiveSpacing(20),
    marginBottom: responsiveSpacing(16),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  statItem: {
    alignItems: 'center',
  },
  statNumber: {
    fontSize: responsiveFontSize(24),
    fontWeight: 'bold',
    color: Colors.text.heading,
    marginBottom: responsiveSpacing(4),
  },
  statLabel: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    textAlign: 'center',
  },
  presentationCard: {
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    padding: responsiveSpacing(20),
    marginBottom: responsiveSpacing(16),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
  },
  presentationTitle: {
    fontSize: responsiveFontSize(18),
    fontWeight: 'bold',
    color: Colors.text.heading,
    marginBottom: responsiveSpacing(12),
  },
  presentationText: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    lineHeight: responsiveFontSize(20),
  },
  messageButton: {
    backgroundColor: Colors.primary.orange,
    borderRadius: 20,
    paddingVertical: responsiveSpacing(16),
    paddingHorizontal: responsiveSpacing(20),
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: responsiveSpacing(16),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 4,
  },
  messageButtonText: {
    fontSize: responsiveFontSize(16),
    fontWeight: '600',
    color: Colors.ui.white,
    marginLeft: responsiveSpacing(8),
  },
});
