import React from 'react';
import { StyleSheet, Text, View } from 'react-native';

import { Colors } from '@/constants/Colors';
import { VolontarLogoProps } from '@/types';
import { responsiveFontSize } from '@/utils/responsive';

export const VolontarLogo: React.FC<VolontarLogoProps> = ({
  size = 80,
  showText = true,
  textColor = Colors.ui.white,
}) => {
  const logoSize = size;
  const iconSize = logoSize * 0.6;
  const textSize = responsiveFontSize(logoSize * 0.3);

  return (
    <View style={styles.container}>
      {/* Logo Icon - Blue square with orange hand */}
      <View style={[
        styles.logoIcon,
        {
          width: logoSize,
          height: logoSize,
          borderRadius: logoSize * 0.2,
        }
      ]}>
        <Text style={[
          styles.handIcon,
          { fontSize: iconSize }
        ]}>
          ✋
        </Text>
      </View>

      {/* Logo Text */}
      {showText && (
        <Text style={[
          styles.logoText,
          {
            fontSize: textSize,
            color: textColor,
            marginTop: logoSize * 0.15,
          }
        ]}>
          Volontar
        </Text>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
  },
  logoIcon: {
    backgroundColor: Colors.primary.blue,
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
  handIcon: {
    color: Colors.primary.orange,
    textAlign: 'center',
  },
  logoText: {
    fontWeight: 'bold',
    textAlign: 'center',
  },
});
