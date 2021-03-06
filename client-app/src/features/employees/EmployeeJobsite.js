import React, { Fragment } from 'react';
import { Link } from 'react-router-dom';
import { Button, Divider, Header, Icon, Popup } from 'semantic-ui-react';
import FilterDateForm from '../../app/layout/FilterDateForm';
import ListJobsiteTimestamps from '../tables/ListJobsiteTimestamps';

// url: '/jobsites/:moniker/:username'
//This page lists all timestamps at a jobsite for a particular user (manager viewing)

const EmployeeJobsite = ({ match }) => {
  const username = match.params.username;
  const moniker = match.params.moniker;
  const pageSize = 1;

  return (
    <Fragment>
      <Header as='h2' color='teal'>
        Timestamps at Jobsite: {moniker}
      </Header>
      <Button color='blue' as={Link} to={`/jobsites/${moniker}`} size='small'>
        See all timestamps
      </Button>
      <Divider />
      <Header
        as='h4'
        color='teal'
        style={{ display: 'inline-block', margin: '5px 5px 20px' }}
      >
        Filtered Employee: <span className='employee-name'> {username}</span>
      </Header>
      <Popup
        trigger={<Icon name='question circle outline' />}
        content={`Showing all timestamps for user ${username} at this jobsite`}
        position='right center'
      />
      <FilterDateForm />

      <ListJobsiteTimestamps
        pageSize={pageSize}
        username={username}
        moniker={moniker}
      />
    </Fragment>
  );
};

export default EmployeeJobsite;
