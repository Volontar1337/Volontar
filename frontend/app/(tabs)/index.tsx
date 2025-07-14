import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import React from 'react';
import { FlatList, StyleSheet, Text, TouchableOpacity, View } from 'react-native';

import { Colors } from '@/constants/Colors';
import { useAuth } from '@/contexts/AuthContext';
import { hp, responsiveFontSize, responsiveSpacing, wp } from '@/utils/responsive';

export default function HomeScreen() {
  const { missions, joinMission, leaveMission, user } = useAuth();

  const handleJoinMission = async (missionId: string) => {
    if (!user) return;
    
    const mission = missions.find(m => m.id === missionId);
    if (!mission) return;
    
    const isJoined = mission.participants.includes(user.id);
    
    if (isJoined) {
      await leaveMission(missionId);
    } else {
      await joinMission(missionId);
    }
  };

  const renderMissionCard = ({ item: mission }: { item: any }) => {
    const isJoined = user ? mission.participants.includes(user.id) : false;
    const spotsLeft = mission.maxParticipants ? mission.maxParticipants - mission.participants.length : null;

    return (
      <View style={styles.missionCard}>
        {/* Mission Image Placeholder */}
        <View style={styles.missionImageContainer}>
          <View style={styles.missionImage}>
            <Ionicons name="image-outline" size={32} color={Colors.ui.gray} />
          </View>
          <View style={styles.missionImageOverlay}>
            <Text style={styles.missionCategory}>Hjälp i hemmet</Text>
          </View>
        </View>

        <View style={styles.missionContent}>
          <View style={styles.missionHeader}>
            <Text style={styles.missionTitle}>{mission.title}</Text>
            <View style={styles.organizationBadge}>
              <Text style={styles.organizationText}>Organisation</Text>
            </View>
          </View>

          <Text style={styles.missionDescription} numberOfLines={3}>
            {mission.description}
          </Text>

          <View style={styles.missionDetails}>
            <Text style={styles.missionLocation}>📍 {mission.location}</Text>
            <Text style={styles.missionDate}>📅 {mission.date} kl {mission.time}</Text>
            <Text style={styles.missionCreator}>👤 {mission.createdByName}</Text>
            {spotsLeft !== null && (
              <Text style={styles.spotsLeft}>
                {spotsLeft > 0 ? `${spotsLeft} platser kvar` : 'Fullt'}
              </Text>
            )}
          </View>

          <View style={styles.missionActions}>
            <TouchableOpacity style={styles.viewMoreButton}>
              <Text style={styles.viewMoreText}>Visa mer</Text>
            </TouchableOpacity>

            <TouchableOpacity
              style={[
                styles.joinButton,
                isJoined ? styles.leaveButton : styles.joinButtonActive,
                spotsLeft === 0 && !isJoined ? styles.joinButtonDisabled : null
              ]}
              onPress={() => handleJoinMission(mission.id)}
              disabled={spotsLeft === 0 && !isJoined}
            >
              <Text style={[
                styles.joinButtonText,
                isJoined ? styles.leaveButtonText : styles.joinButtonActiveText
              ]}>
                {isJoined ? 'Lämna' : spotsLeft === 0 ? 'Fullt' : 'Anmäl dig'}
              </Text>
            </TouchableOpacity>
          </View>
        </View>
      </View>
    );
  };

  return (
    <LinearGradient
      colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
      style={styles.container}
    >
      {/* Header with gradient */}
      <LinearGradient
        colors={[Colors.gradient.start, Colors.gradient.middle, Colors.gradient.end]}
        style={styles.header}
      >
        <Text style={styles.headerTitle}>Uppdrag</Text>
        <Text style={styles.headerSubtitle}>Hitta uppdrag att hjälpa till med</Text>
      </LinearGradient>

      {/* Mission List */}
      <FlatList
        data={missions}
        renderItem={renderMissionCard}
        keyExtractor={(item) => item.id}
        style={styles.missionList}
        contentContainerStyle={styles.missionListContent}
        showsVerticalScrollIndicator={false}
        ListEmptyComponent={
          <View style={styles.emptyState}>
            <Ionicons name="list-outline" size={48} color={Colors.ui.gray} />
            <Text style={styles.emptyStateText}>Inga uppdrag tillgängliga</Text>
            <Text style={styles.emptyStateSubtext}>
              Nya uppdrag kommer att visas här när de skapas
            </Text>
          </View>
        }
        ListHeaderComponent={
          <View style={styles.listHeader}>
            <Text style={styles.resultsCount}>
              {missions.length} uppdrag tillgängliga
            </Text>
          </View>
        }
      />
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
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: responsiveFontSize(24),
    fontWeight: 'bold',
    color: Colors.text.heading,
    marginBottom: responsiveSpacing(8),
    textAlign: 'center',
  },
  headerSubtitle: {
    fontSize: responsiveFontSize(16),
    color: Colors.text.body,
    textAlign: 'center',
  },
  missionList: {
    flex: 1,
  },
  missionListContent: {
    paddingHorizontal: wp(5),
    paddingBottom: hp(10), // Space for tab bar
    maxWidth: 800,
    alignSelf: 'center',
    width: '100%',
  },
  listHeader: {
    paddingVertical: hp(2),
  },
  resultsCount: {
    fontSize: responsiveFontSize(16),
    fontWeight: '600',
    color: Colors.text.heading,
  },
  missionCard: {
    backgroundColor: Colors.ui.white,
    borderRadius: 20,
    marginBottom: responsiveSpacing(16),
    shadowColor: Colors.ui.shadow,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
    overflow: 'hidden',
  },
  missionImageContainer: {
    position: 'relative',
    height: 120,
  },
  missionImage: {
    flex: 1,
    backgroundColor: Colors.ui.lightGray,
    alignItems: 'center',
    justifyContent: 'center',
  },
  missionImageOverlay: {
    position: 'absolute',
    top: responsiveSpacing(12),
    left: responsiveSpacing(12),
  },
  missionCategory: {
    backgroundColor: Colors.primary.orange,
    color: Colors.ui.white,
    fontSize: responsiveFontSize(12),
    fontWeight: '600',
    paddingHorizontal: responsiveSpacing(8),
    paddingVertical: responsiveSpacing(4),
    borderRadius: 12,
    overflow: 'hidden',
  },
  missionContent: {
    padding: responsiveSpacing(16),
  },
  missionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: responsiveSpacing(8),
  },
  missionTitle: {
    fontSize: responsiveFontSize(18),
    fontWeight: 'bold',
    color: Colors.text.heading,
    flex: 1,
    marginRight: responsiveSpacing(8),
  },
  organizationBadge: {
    backgroundColor: Colors.ui.lightGray,
    borderRadius: 12,
    paddingHorizontal: responsiveSpacing(8),
    paddingVertical: responsiveSpacing(4),
  },
  organizationText: {
    fontSize: responsiveFontSize(12),
    color: Colors.text.body,
    fontWeight: '600',
  },
  missionDescription: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    lineHeight: responsiveFontSize(20),
    marginBottom: responsiveSpacing(12),
  },
  missionDetails: {
    marginBottom: responsiveSpacing(16),
  },
  missionLocation: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    marginBottom: responsiveSpacing(4),
  },
  missionDate: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    marginBottom: responsiveSpacing(4),
  },
  missionCreator: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    marginBottom: responsiveSpacing(4),
  },
  spotsLeft: {
    fontSize: responsiveFontSize(14),
    color: Colors.primary.orange,
    fontWeight: '600',
  },
  missionActions: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  viewMoreButton: {
    paddingVertical: responsiveSpacing(8),
    paddingHorizontal: responsiveSpacing(12),
  },
  viewMoreText: {
    fontSize: responsiveFontSize(14),
    color: Colors.primary.blue,
    fontWeight: '600',
    textDecorationLine: 'underline',
  },
  joinButton: {
    borderRadius: 20,
    paddingVertical: responsiveSpacing(10),
    paddingHorizontal: responsiveSpacing(16),
    minWidth: 100,
    alignItems: 'center',
  },
  joinButtonActive: {
    backgroundColor: Colors.primary.blue,
  },
  leaveButton: {
    backgroundColor: Colors.status.error,
  },
  joinButtonDisabled: {
    backgroundColor: Colors.ui.lightGray,
  },
  joinButtonText: {
    fontSize: responsiveFontSize(14),
    fontWeight: '600',
  },
  joinButtonActiveText: {
    color: Colors.ui.white,
  },
  leaveButtonText: {
    color: Colors.ui.white,
  },
  emptyState: {
    alignItems: 'center',
    paddingVertical: hp(8),
  },
  emptyStateText: {
    fontSize: responsiveFontSize(18),
    fontWeight: 'bold',
    color: Colors.text.body,
    textAlign: 'center',
    marginTop: responsiveSpacing(16),
  },
  emptyStateSubtext: {
    fontSize: responsiveFontSize(14),
    color: Colors.text.body,
    textAlign: 'center',
    marginTop: responsiveSpacing(8),
    paddingHorizontal: wp(5),
  },
});
