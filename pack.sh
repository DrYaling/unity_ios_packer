#!/bin/bash

#for debug
#请将 TARGET-NAME 改成项目的 TARGET 名称
#放置于 .xcodeproj 同级目录中执行
echo ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>"
basepath=$(cd `dirname $0`; pwd)
cd $basepath
echo $(pwd)
xcodePath='$pwd+/'
target_name="Unity-iPhone"
tmp_dir="tmp"
#profile="xxx-xxx-xxx-xxx-xxx"
#profile_name="xx-Dev"
#sign="iPhone Developer: xxxx"
TIMESTAMP=`date +"%Y%m%d%H%M"`
#ipa=ipaName_debug.$TIMESTAMP.ipa
 
#rm -rf $tmp_dir
#rm $ipa
 
#xcodebuild -target $target_name -sdk iphoneos -configuration Debug ARCHS="armv7 armv7s armv64" CONFIGURATION_BUILD_DIR=$tmp_dir PROVISIONING_PROFILE="$profile"
 
#xcodebuild -scheme $target_name -destination generic/platform=iOS archive CODE_SIGN_IDENTITY="$sign" -archivePath $tmp_dir/target.xcarchive PROVISIONING_PROFILE="$profile"
 
#xcodebuild -exportArchive -exportFormat IPA -exportProvisioningProfile "$profile_name" -archivePath $tmp_dir/target.xcarchive/ -exportPath ./$ipa
 
#rm -rf $tmp_dir
 
 
   
  
#for relese
 
profile="c6a9a426-db5c-491b-9c3e-7f671cba3a91"
profile_name="anyapp"
sign="iPhone Developer: Jian Wang (YS76ACJQS4)"
ipa=街头篮球_release.$TIMESTAMP.ipa
#arcs="armv7 armv7s armv64"
arcs="arm64"
#rm -rf $tmp_dir
#rm $ipa
 
xcodebuild -target $target_name -sdk iphoneos -configuration Release ARCHS=$arcs CONFIGURATION_BUILD_DIR=$tmp_dir PROVISIONING_PROFILE="$profile"
 
xcodebuild -scheme $target_name -destination generic/platform=iOS archive CODE_SIGN_IDENTITY="$sign" -archivePath $tmp_dir/target.xcarchive PROVISIONING_PROFILE="$profile"
 
xcodebuild -exportArchive -exportFormat IPA -exportProvisioningProfile "$profile_name" -archivePath $tmp_dir/target.xcarchive/ -exportPath ./$ipa
source dSYMUpload.sh "1105410169" "iwltSl2URuO7O6EE" "com.uminton.freestyle" "0.9.4.0" $basepath $basepath 1

echo "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<"
#rm -rf $tmp_dir